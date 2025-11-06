using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
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

        // 🌐 API URL (dùng bản mới Gemini 2.0)
        private readonly string _geminiUrl =
            "https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent";

        // 🛍️ Từ khóa kích hoạt tìm sản phẩm
        private readonly List<string> _productTriggers = new()
        {
            "sản phẩm", "mua", "giá", "bán", "điện thoại", "laptop", "tai nghe", "phụ kiện",
            "tivi", "máy tính", "chuột", "bàn phím", "gaming", "macbook", "iphone", "android"
        };

        // 📦 [NÂNG CẤP] Từ khóa kích hoạt hỏi tồn kho/số lượng
        private readonly List<string> _inventoryTriggers = new()
        {
            "còn bao nhiêu", "số lượng", "hết hàng", "bao nhiêu", "hiện có"
        };

        // 🏪 Từ khóa kích hoạt giới thiệu shop
        private readonly List<string> _introTriggers = new()
        {
            "shop", "cửa hàng", "giới thiệu", "bạn là ai", "ở đâu", "uy tín", "thông tin", "chính sách"
        };

        // 🧾 Mô tả shop có thể load từ DB hoặc config
        private readonly string _shopIntro = @"
Xin chào 👋! Mình là **Trợ lý ảo EcomBot**, đại diện cho cửa hàng **TechStore** 💎 
🛒 *EcommerceX* chuyên cung cấp các sản phẩm **điện tử, laptop, điện thoại, phụ kiện chính hãng** với giá cực tốt. 
⚡ Ưu điểm:
- Bảo hành 12 tháng toàn quốc 
- Giao hàng nhanh 2h nội thành 
- Hỗ trợ trả góp 0% lãi suất 
- CSKH tận tâm 24/7 

Bạn có thể hỏi mình bất kỳ điều gì như:
👉 “Shop có iPhone 15 không?”
👉 “Laptop chơi game tầm 20 triệu có không?”
👉 “Chính sách bảo hành thế nào?”
";

        // 🧩 System prompt để Gemini hiểu vai trò của AI
        private readonly string _systemPrompt = @"
Bạn là trợ lý ảo **EcomBot** của cửa hàng **TechStore**.
Nhiệm vụ của bạn là:
- **Tư vấn và bán hàng** cho các sản phẩm điện tử, laptop, điện thoại, phụ kiện.
- Trả lời thân thiện, chuyên nghiệp, luôn xưng 'mình' hoặc 'EcomBot'.
- Nếu người dùng hỏi các câu như 'Lọc theo giá' hay 'Samsung', hãy **khuyến khích họ dùng chức năng tìm kiếm sản phẩm** của shop (vì bạn chỉ là AI, bạn không có quyền truy cập trực tiếp vào cơ sở dữ liệu để lọc chuyên sâu).
- Nếu người dùng hỏi ngoài phạm vi công nghệ/sản phẩm, hãy từ chối lịch sự.
- Luôn khuyến khích truy cập website: https://ecommercex.vn
";

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

        public async Task<ChatResponseDto> ProcessUserMessageAsync(ChatRequestDto request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return new ChatResponseDto { BotMessage = "Hãy nhập điều bạn muốn hỏi nhé 😊" };

            string message = request.Message.Trim().ToLowerInvariant();

            // ===================== 🏪 GIỚI THIỆU SHOP =====================
            if (_introTriggers.Any(k => message.Contains(k)))
            {
                _logger.LogInformation("✨ Người dùng hỏi về shop → trả lời giới thiệu.");
                return new ChatResponseDto { BotMessage = _shopIntro };
            }

            // ===================== 📦 KIỂM TRA TỒN KHO =====================
            if (_inventoryTriggers.Any(k => message.Contains(k)))
            {
                _logger.LogInformation("📦 Kích hoạt logic kiểm tra tồn kho cho: {Message}", message);

                // Loại bỏ các trigger để lấy từ khóa tìm kiếm
                string searchKeyword = request.Message.Trim();
                _inventoryTriggers.ForEach(t => searchKeyword = searchKeyword.Replace(t, "", StringComparison.OrdinalIgnoreCase));
                searchKeyword = searchKeyword.Trim();

                if (string.IsNullOrWhiteSpace(searchKeyword))
                {
                    return new ChatResponseDto { BotMessage = "Bạn muốn mình kiểm tra số lượng của sản phẩm nào nhỉ? 💬" };
                }

                int count = await _chatQueries.GetProductCountAsync(searchKeyword);

                if (count > 0)
                {
                    string reply = $"Chào bạn! Hiện tại, mình tìm thấy **{count}** loại sản phẩm phù hợp với từ khóa '{searchKeyword}' trong kho đó! 🎉 Bạn có muốn mình liệt kê danh sách không?";
                    return new ChatResponseDto { BotMessage = reply };
                }

                return new ChatResponseDto
                {
                    BotMessage = $"Mình không tìm thấy sản phẩm nào liên quan đến '{searchKeyword}' trong kho 😢. Bạn thử mô tả tên sản phẩm chi tiết hơn nhé!"
                };
            }

            // ===================== 🛍️ TÌM KIẾM SẢN PHẨM =====================
            if (_productTriggers.Any(k => message.Contains(k)))
            {
                _logger.LogInformation("🛒 Kích hoạt logic tìm sản phẩm cho: {Message}", message);
                var products = await _chatQueries.SearchProductsAsync(request.Message);

                if (products != null && products.Any())
                {
                    var formatted = string.Join("\n",
                        products.Take(5).Select(p => $"- {p.Name} ({p.Price:N0}₫)"));

                    string responseText =
                        $"Mình tìm thấy {products.Count} sản phẩm phù hợp với yêu cầu của bạn 👇\n{formatted}\n\n" +
                        "Bạn muốn mình lọc theo thương hiệu hay mức giá không? Hoặc bạn có thể truy cập website để xem chi tiết hơn: https://ecommercex.vn 💬";

                    return new ChatResponseDto
                    {
                        BotMessage = responseText,
                        Products = products
                    };
                }

                return new ChatResponseDto
                {
                    BotMessage = "Mình không tìm thấy sản phẩm nào phù hợp 😢. Bạn có thể thử mô tả chi tiết hơn không?"
                };
            }

            // ===================== 💬 GỌI GEMINI CHO HỎI ĐÁP KHÁC =====================
            var body = new
            {
                contents = new[]
                {
                    new { role = "system", parts = new[] { new { text = _systemPrompt } } },
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
                    _logger.LogError("❌ Gemini API Error: {StatusCode} - {Response}", response.StatusCode, responseText);
                    return new ChatResponseDto
                    {
                        BotMessage = "Xin lỗi 😔, hệ thống đang bận xử lý. Bạn thử lại sau nhé!"
                    };
                }

                using var doc = JsonDocument.Parse(responseText);
                var root = doc.RootElement;

                // Xử lý Parse JSON Response từ Gemini
                string reply = root
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString() ?? "Xin lỗi, mình chưa hiểu rõ lắm 😅.";

                return new ChatResponseDto { BotMessage = reply };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 Lỗi khi gọi Gemini API.");
                return new ChatResponseDto
                {
                    BotMessage = "Hệ thống đang bận chút xíu 😅. Bạn vui lòng thử lại sau nhé!"
                };
            }
        }
    }
}