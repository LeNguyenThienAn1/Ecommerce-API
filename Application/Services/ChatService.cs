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

        // 🌐 API URL (using the new Gemini 2.0 version)
        private readonly string _geminiUrl =
            "https://generativelanguage.googleapis.com/v1/models/gemini-2.0-flash:generateContent";

        // 🛍️ Keywords to trigger product search
        private readonly List<string> _productTriggers = new()
        {
            // Core keywords
            "product", "item", "goods", "merchandise", "model",
            // Actions/Price
            "buy", "purchase", "cost", "price", "sell", "discount", "promotion", "deal", "cheap", "expensive",
            // Categories/Names
            "phone", "mobile", "smartphone", "laptop", "pc", "computer", "headphone", "earphone", "accessory",
            "tv", "television", "mouse", "keyboard", "webcam", "monitor", "speaker",
            // Brands/Types
            "gaming", "macbook", "iphone", "android", "samsung", "dell", "asus", "xiaomi", "oppo"
        };

        // 📦 [UPGRADE] Keywords to trigger inventory/stock check
        private readonly List<string> _inventoryTriggers = new()
        {
            // Quantity/Availability
            "how many left", "quantity", "number of", "count", "stock", "availability", "available", "is there", "do you have",
            // Status
            "out of stock", "sold out", "in stock", "currently available", "have any", "remaining"
        };

        // 🏪 Keywords to trigger shop introduction
        private readonly List<string> _introTriggers = new()
        {
            // Shop/Identity
            "shop", "store", "company", "business", "who are you", "what are you", "what is this",
            // Location/Info
            "where are you", "address", "location", "contact", "phone number", "hours", "open", "close",
            // Trust/Policies
            "reputable", "info", "information", "policy", "warranty", "guarantee", "return", "shipping", "delivery", "trust"
        };

        // 🧾 Shop description (can be loaded from DB or config)
        private readonly string _shopIntro = @"
Hello 👋! I am **EcomBot**, the virtual assistant for **TechStore** 💎
🛒 *EcommerceX* specializes in providing **genuine electronics, laptops, phones, and accessories** at great prices.
⚡ Advantages:
- 12-month nationwide warranty
- 2-hour express delivery in the city
- 0% installment plan support
- Dedicated 24/7 customer service

You can ask me anything, such as:
👉 ""Does the shop have iPhone 15?""
👉 ""Do you have a gaming laptop around 20 million?""
👉 ""What is the warranty policy?""
";

        // 🧩 System prompt to help Gemini understand the AI's role
        private readonly string _systemPrompt = @"
You are the virtual assistant **EcomBot** for the **TechStore** shop.
Your mission is to:
- **Consult and sell** electronics, laptops, phones, and accessories.
- Reply in a friendly, professional manner, always referring to yourself as 'I' or 'EcomBot'.
- If the user asks questions like 'Filter by price' or 'Samsung', you should **encourage them to use the shop's product search function** (because you are an AI, you do not have direct access to the database for advanced filtering).
- If the user asks about topics outside the scope of technology/products, politely decline.
- Always encourage visiting the website: https://ecommercex.vn
";

        public ChatService(HttpClient httpClient, IChatQueries chatQueries, IConfiguration configuration, ILogger<ChatService> logger)
        {
            _httpClient = httpClient;
            _chatQueries = chatQueries;
            _logger = logger;

            _geminiApiKey = configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(_geminiApiKey))
            {
                _logger.LogCritical("❌ Configuration error: Gemini API key is missing in appsettings.json.");
                throw new Exception("Gemini API key is missing in appsettings.json");
            }
        }

        public async Task<ChatResponseDto> ProcessUserMessageAsync(ChatRequestDto request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return new ChatResponseDto { BotMessage = "Please enter what you would like to ask 😊" };

            string message = request.Message.Trim().ToLowerInvariant();

            // ===================== 🏪 SHOP INTRODUCTION =====================
            if (_introTriggers.Any(k => message.Contains(k)))
            {
                _logger.LogInformation("✨ User asks about the shop -> replying with introduction.");
                return new ChatResponseDto { BotMessage = _shopIntro };
            }

            // ===================== 📦 INVENTORY CHECK =====================
            if (_inventoryTriggers.Any(k => message.Contains(k)))
            {
                _logger.LogInformation("📦 Triggering inventory check logic for: {Message}", message);

                // Remove triggers to get the search keyword
                string searchKeyword = request.Message.Trim();
                _inventoryTriggers.ForEach(t => searchKeyword = searchKeyword.Replace(t, "", StringComparison.OrdinalIgnoreCase));
                searchKeyword = searchKeyword.Trim();

                if (string.IsNullOrWhiteSpace(searchKeyword))
                {
                    return new ChatResponseDto { BotMessage = "Which product would you like me to check the quantity for? 💬" };
                }

                int count = await _chatQueries.GetProductCountAsync(searchKeyword);

                if (count > 0)
                {
                    string reply = $"Hello! Currently, I found **{count}** product types matching the keyword '{searchKeyword}' in stock! 🎉 Would you like me to list them?";
                    return new ChatResponseDto { BotMessage = reply };
                }

                return new ChatResponseDto
                {
                    BotMessage = $"I couldn't find any products related to '{searchKeyword}' in stock 😢. Could you try describing the product name in more detail?"
                };
            }

            // ===================== 🛍️ PRODUCT SEARCH =====================
            if (_productTriggers.Any(k => message.Contains(k)))
            {
                _logger.LogInformation("🛒 Triggering product search logic for: {Message}", message);
                var products = await _chatQueries.SearchProductsAsync(request.Message);

                if (products != null && products.Any())
                {
                    var formatted = string.Join("\n",
                        products.Take(5).Select(p => $"- {p.Name} ({p.Price:N0}₫)"));

                    string responseText =
                        $"I found {products.Count} products matching your request 👇\n{formatted}\n\n" +
                        "Would you like me to filter by brand or price range? Or you can visit the website for more details: https://ecommercex.vn 💬";

                    return new ChatResponseDto
                    {
                        BotMessage = responseText,
                        Products = products
                    };
                }

                return new ChatResponseDto
                {
                    BotMessage = "I couldn't find any suitable products 😢. Could you try describing it in more detail?"
                };
            }

            // ===================== 💬 CALL GEMINI FOR OTHER QUERIES =====================
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
                        BotMessage = "I apologize 😔, the system is currently busy. Please try again later!"
                    };
                }

                using var doc = JsonDocument.Parse(responseText);
                var root = doc.RootElement;

                // Process Parse JSON Response from Gemini
                string reply = root
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString() ?? "I'm sorry, I don't quite understand 😅.";

                return new ChatResponseDto { BotMessage = reply };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 Error calling Gemini API.");
                return new ChatResponseDto
                {
                    BotMessage = "The system is a little busy right now 😅. Please try again later!"
                };
            }
        }
    }
}