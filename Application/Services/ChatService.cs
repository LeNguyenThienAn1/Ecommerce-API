using System.Net.Http.Json;
using Application.DTOs;
using EntityHandler.Queries.Interface;
using EntityHandler.Services.Interface;
using Microsoft.Extensions.Configuration;

namespace EntityHandler.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatQueries _chatQueries;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public ChatService(IChatQueries chatQueries, IConfiguration config, HttpClient httpClient)
        {   
            _chatQueries = chatQueries;
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<ChatResponseDto> ProcessUserMessageAsync(ChatRequestDto request)
        {
            // 1. Gửi message sang Gemini API
            var apiKey = _config["Gemini:ApiKey"]; // lưu trong appsettings.json
            var apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={apiKey}";

            var payload = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = request.Message } } }
                }
            };

            var response = await _httpClient.PostAsJsonAsync(apiUrl, payload);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<dynamic>();
            string botMessage = json?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"] ?? "Xin lỗi, tôi chưa hiểu yêu cầu.";

            // 2. Query DB dựa trên message user
            var products = await _chatQueries.SearchProductsAsync(request.Message);

            // 3. Trả kết quả
            return new ChatResponseDto
            {
                BotMessage = botMessage,
                Products = products
            };
        }
    }
}
