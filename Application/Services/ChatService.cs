using System.Net.Http;
using System.Text;
using System.Text.Json;
using Application.DTOs;
using EntityHandler.Queries.Interface;
using EntityHandler.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EntityHandler.Services
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IChatQueries _chatQueries;
        private readonly ILogger<ChatService> _logger;
        private readonly string _geminiApiKey;
        private readonly string _geminiUrl =
    "https://generativelanguage.googleapis.com/v1/models/gemini-2.5-pro:generateContent";

        // 💡 DANH SÁCH TỪ KHÓA ĐỂ KÍCH HOẠT TÌM KIẾM SẢN PHẨM (Không phải Stopwords)
        private readonly List<string> _productTriggers = new List<string>
        {
            "sản phẩm", "mua", "giá", "điện tử", "điện thoại", "laptop",
            "tv", "máy tính", "tai nghe", "bàn phím", "chuột", "tìm", "cần"
        };

        // 🛑 DANH SÁCH TỪ DỪNG (STOPWORDS) PHỔ BIẾN TIẾNG VIỆT
        // Dùng để làm sạch chuỗi tìm kiếm trước khi gửi xuống database
        private readonly HashSet<string> _vietnameseStopwords = new HashSet<string>
        {
            "tôi", "muốn", "cần", "một", "chiếc", "cái", "nào", "gì", "nhất",
            "và", "là", "tìm", "kiếm", "về", "loại", "với", "cho", "xin", "làm",
            "có", "hay", "được", "rồi", "nữa", "những"
        };


        public ChatService(HttpClient httpClient, IChatQueries chatQueries, IConfiguration configuration, ILogger<ChatService> logger)
        {
            _httpClient = httpClient;
            _chatQueries = chatQueries;
            _logger = logger;

            _geminiApiKey = configuration["Gemini:ApiKey"];

            if (string.IsNullOrEmpty(_geminiApiKey))
            {
                _logger.LogCritical("❌ Cấu hình lỗi: Gemini API key bị thiếu trong appsettings.json.");
                throw new Exception("Gemini API key is missing in appsettings.json");
            }
        }

        /// <summary>
        /// Loại bỏ các từ dừng và chỉ giữ lại từ khóa quan trọng để tìm kiếm database.
        /// </summary>
        private string ExtractKeywords(string message)
        {
            // Tách tin nhắn thành các từ
            var words = message.Split(new[] { ' ', ',', '.', '?', '!' }, StringSplitOptions.RemoveEmptyEntries);

            // Loại bỏ stopwords và nối lại thành chuỗi tìm kiếm
            var relevantWords = words
                .Where(word => !_vietnameseStopwords.Contains(word.ToLowerInvariant()))
                .ToList();

            // Nếu không còn từ nào, dùng lại toàn bộ tin nhắn gốc (đề phòng trường hợp lỗi)
            if (!relevantWords.Any())
            {
                return message;
            }

            // Trả về chuỗi mới, ví dụ: "mua điện thoại samsung" -> "điện thoại samsung"
            return string.Join(" ", relevantWords);
        }

        public async Task<ChatResponseDto> ProcessUserMessageAsync(ChatRequestDto request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return new ChatResponseDto { BotMessage = "Xin vui lòng nhập tin nhắn." };
            }

            string message = request.Message.Trim().ToLowerInvariant();

            // ===================== 🛍️ PHÂN LOẠI VÀ TÌM KIẾM SẢN PHẨM =====================

            bool isProductSearch = _productTriggers.Any(k => message.Contains(k));

            if (isProductSearch)
            {
                _logger.LogInformation("🚀 Kích hoạt logic tìm kiếm sản phẩm cho tin nhắn: {Message}", request.Message);

                // ✅ SỬ DỤNG HÀM MỚI ĐỂ LÀM SẠCH TỪ KHÓA
                string cleanedKeyword = ExtractKeywords(message);

                _logger.LogDebug("Từ khóa đã làm sạch: {Keyword}", cleanedKeyword);

                // Dùng từ khóa đã làm sạch để tìm kiếm database
                var products = await _chatQueries.SearchProductsAsync(cleanedKeyword);

                if (products != null && products.Any())
                {
                    // Logic trả về danh sách sản phẩm
                    var formattedList = string.Join("\n",
                        products.Select(p => $"- {p.Name} ({p.Price:N0}₫)"));

                    return new ChatResponseDto
                    {
                        BotMessage = $"Mình tìm thấy {products.Count} sản phẩm liên quan:\n{formattedList}",
                        Products = products
                    };
                }

                return new ChatResponseDto
                {
                    BotMessage = "Xin lỗi, mình không tìm thấy sản phẩm nào phù hợp với yêu cầu của bạn."
                };
            }

            // ===================== 💬 XỬ LÝ BẰNG GEMINI API =====================

            var body = new
            {
                contents = new[]
                {
                    new { role = "user", parts = new[] { new { text = request.Message } } }
                }
            };

            var json = JsonSerializer.Serialize(body);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_geminiUrl}?key={_geminiApiKey}", httpContent);
                var responseText = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("❌ Gemini API Error: Status {StatusCode}. Response: {Response}", response.StatusCode, responseText);
                    return new ChatResponseDto
                    {
                        BotMessage = $"Lỗi từ Gemini API ({response.StatusCode}). Vui lòng kiểm tra API Key và Quota."
                    };
                }

                using var doc = JsonDocument.Parse(responseText);
                var root = doc.RootElement;

                // Logic phân tích JSON an toàn... (Giữ nguyên như đã sửa lần trước)

                if (root.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    var firstCandidate = candidates[0];

                    if (firstCandidate.TryGetProperty("finishReason", out var finishReason) &&
                        finishReason.GetString() == "SAFETY")
                    {
                        return new ChatResponseDto { BotMessage = "Xin lỗi, câu hỏi của bạn không vượt qua được bộ lọc an toàn của AI." };
                    }

                    if (firstCandidate.TryGetProperty("content", out var content) &&
                        content.TryGetProperty("parts", out var parts) &&
                        parts.GetArrayLength() > 0 &&
                        parts[0].TryGetProperty("text", out var textElement))
                    {
                        var reply = textElement.GetString();
                        return new ChatResponseDto { BotMessage = reply };
                    }
                }

                return new ChatResponseDto { BotMessage = "Xin lỗi, tôi gặp sự cố khi xử lý câu trả lời." };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 Lỗi không xác định khi xử lý tin nhắn.");
                return new ChatResponseDto { BotMessage = "Xin lỗi, hệ thống AI đang bận (Lỗi mạng hoặc lỗi nội bộ không xác định). Vui lòng thử lại sau." };
            }
        }
    }
}