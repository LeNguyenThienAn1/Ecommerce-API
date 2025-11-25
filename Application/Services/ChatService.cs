using Application.DTOs;
using EntityHandler.Queries.Interface;
using EntityHandler.Services.Interface;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EntityHandler.Services
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IChatQueries _chatQueries;
        private readonly ILogger<ChatService> _logger;
        private readonly string _geminiApiKey;

        // FIX: Changed 'gemini-1.5-flash-latest' to 'gemini-2.5-flash' for better compatibility with v1beta
        private readonly string _geminiUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        private readonly string _systemPrompt = @"
        You are the virtual assistant **EcomBot** for the **TechStore** shop.
        Your mission is to:
        - Assist users in finding products in the electronics store.
        - Use the available tools to answer questions about products, prices, and stock.
        - Be friendly, professional, and always refer to yourself as 'I' or 'EcomBot'.
        - When presenting products, be concise and clear.
        - If you find products, tell the user how many you found and offer to show more details.
        - If you can't find a product, apologize and suggest trying different keywords.
        - If the user asks about topics outside the scope of technology/products, politely decline and steer the conversation back to products.
        - Always encourage visiting the website: https://ecommercex.vn for more details.
        ";

        // Keep the intro for simple greetings
        private readonly string _shopIntro = @"
        Hello 👋! I am **EcomBot**, the virtual assistant for **TechStore** 💎
        ... (rest of the intro)
        ";

        public ChatService(HttpClient httpClient, IChatQueries chatQueries, IConfiguration configuration, ILogger<ChatService> logger)
        {
            _httpClient = httpClient;
            _chatQueries = chatQueries;
            _logger = logger;
            _geminiApiKey = configuration["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(_geminiApiKey))
            {
                _logger.LogCritical("❌ Configuration error: Gemini API key is missing.");
                throw new InvalidOperationException("Gemini API key is not configured.");
            }
        }

        public async Task<ChatResponseDto> ProcessUserMessageAsync(ChatRequestDto request, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(request.Message))
                return new ChatResponseDto { BotMessage = "Please enter something to ask. 😊" };

            // Simple trigger for introduction
            string lowerMessage = request.Message.Trim().ToLowerInvariant();
            if (new[] { "shop", "store", "who are you", "what is this" }.Any(s => lowerMessage.Contains(s)))
            {
                _logger.LogInformation("✨ User asked about the shop -> replying with introduction.");
                return new ChatResponseDto { BotMessage = _shopIntro };
            }

            try
            {
                // Step 1: Call Gemini with the user's query and tool definitions
                var (firstResponse, products) = await CallGeminiAndExecuteTools(request.Message);

                // If tools were executed, the products are already in the DTO
                if (products != null && products.Any())
                {
                    return new ChatResponseDto { BotMessage = firstResponse, Products = products };
                }

                // If no tool was called, return the direct response
                return new ChatResponseDto { BotMessage = firstResponse };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🚨 An error occurred while processing the chat message.");
                return new ChatResponseDto
                {
                    BotMessage = "I'm having a little trouble right now. Please try again later. 😅"
                };
            }
        }

        private async Task<(string Response, List<ProductDto>? Products)> CallGeminiAndExecuteTools(string userMessage)
        {
            var requestBody = BuildInitialGeminiRequest(userMessage);
            string jsonRequest = requestBody.ToJsonString();

            var geminiResponse = await PostToGeminiAsync(jsonRequest);

            var functionCall = GetFunctionCall(geminiResponse);

            if (functionCall == null)
            {
                // No function call, just return the text content
                string? content = GetTextContent(geminiResponse) ?? "I'm not sure how to respond to that. Could you try asking differently?";
                return (content, null);
            }

            // Step 2: Execute the function requested by the model
            var (toolResponse, products) = await ExecuteTool(functionCall);

            // Step 3: Call Gemini again with the tool's result
            var secondRequestBody = BuildSecondGeminiRequest(userMessage, functionCall, toolResponse);
            string secondJsonRequest = secondRequestBody.ToJsonString();

            var finalGeminiResponse = await PostToGeminiAsync(secondJsonRequest);

            string finalAnswer = GetTextContent(finalGeminiResponse) ?? "I've processed the information. Here are the results.";

            // Return the final text from Gemini and the products found by the tool
            return (finalAnswer, products);
        }

        private async Task<(JsonNode ToolResponse, List<ProductDto>? Products)> ExecuteTool(JsonNode functionCall)
        {
            string functionName = functionCall["name"]!.GetValue<string>();
            JsonNode? args = functionCall["args"];
            _logger.LogInformation("Executing tool: {FunctionName}", functionName);

            List<ProductDto>? products = null;
            object result = new { };

            switch (functionName)
            {
                case "find_products_by_criteria":
                    var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
                    var criteria = JsonSerializer.Deserialize<ProductSearchCriteria>(args!.ToJsonString(), options);
                    products = await _chatQueries.FindProductsByCriteriaAsync(
                        criteria.keywords, criteria.category, criteria.brand,
                        criteria.minPrice, criteria.maxPrice, criteria.sortBy);
                    result = new { products = products ?? new List<ProductDto>() };
                    break;

                case "get_product_count":
                    string? keyword = args?["keyword"]?.GetValue<string>();
                    int count = await _chatQueries.GetProductCountAsync(keyword ?? "");
                    result = new { count };
                    break;

                default:
                    result = new { error = $"Unknown function: {functionName}" };
                    break;
            }

                                                // The result needs to be packaged in a specific structure for Gemini

                                                var toolResponse = new JsonObject

                                                {

                                                    ["name"] = functionName,

                                                    ["response"] = JsonSerializer.SerializeToNode(result)

                                                };

                                                

                                                return (toolResponse, products);

                                            }

                                    

                                    

                                            private async Task<JsonNode> PostToGeminiAsync(string jsonPayload)

                                            {

                                                var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                                                var response = await _httpClient.PostAsync($"{_geminiUrl}?key={_geminiApiKey}", httpContent);

                                                var responseText = await response.Content.ReadAsStringAsync();

                                    

                                                if (!response.IsSuccessStatusCode)

                                                {

                                                    _logger.LogError("❌ Gemini API Error: {StatusCode} - {Response}", response.StatusCode, responseText);

                                                    throw new HttpRequestException($"Gemini API request failed with status code {response.StatusCode}.");

                                                }

                                                

                                                return JsonNode.Parse(responseText)!;

                                            }

                                            

                                            // Helper methods and classes for building requests

                                            private JsonObject BuildInitialGeminiRequest(string userMessage)

                                            {

                                                return new JsonObject

                                                {

                                                    ["system_instruction"] = new JsonObject

                                                    {

                                                        ["parts"] = new JsonArray { new JsonObject { ["text"] = _systemPrompt } }

                                                    },

                                                    ["contents"] = new JsonArray

                                                    {

                                                        new JsonObject

                                                        {

                                                            ["role"] = "user",

                                                            ["parts"] = new JsonArray { new JsonObject { ["text"] = userMessage } }

                                                        }

                                                    },

                                                    ["tools"] = new JsonArray { new JsonObject { ["function_declarations"] = GetFunctionDeclarations() } }

                                                };

                                            }

                                            

                                            private JsonObject BuildSecondGeminiRequest(string userMessage, JsonNode functionCall, JsonNode toolResponse)

                                            {

                                                 return new JsonObject

                                                 {

                                                    ["system_instruction"] = new JsonObject

                                                    {

                                                        ["parts"] = new JsonArray { new JsonObject { ["text"] = _systemPrompt } }

                                                    },

                                                    ["contents"] = new JsonArray

                                                    {

                                                        // User's original message

                                                        new JsonObject

                                                        {

                                                            ["role"] = "user",

                                                            ["parts"] = new JsonArray { new JsonObject { ["text"] = userMessage } }

                                                        },

                                                        // The function call Gemini wanted to make

                                                        new JsonObject

                                                        {

                                                            ["role"] = "model",

                                                            ["parts"] = new JsonArray { new JsonObject { ["function_call"] = functionCall.DeepClone() } }

                                                        },

                                                        // The result of our tool execution

                                                         new JsonObject

                                                        {

                                                            ["role"] = "tool",

                                                            ["parts"] = new JsonArray { new JsonObject { ["functionResponse"] = toolResponse.DeepClone() } }

                                                        }

                                                    }

                                                 };

                                            }        private JsonArray GetFunctionDeclarations()
        {
            return new JsonArray
            {
                new JsonObject
                {
                    ["name"] = "find_products_by_criteria",
                    ["description"] = "Searches for products based on various criteria like keywords, category, brand, price range, and sorting order.",
                    ["parameters"] = new JsonObject
                    {
                        ["type"] = "OBJECT",
                        ["properties"] = new JsonObject
                        {
                            ["keywords"] = new JsonObject { ["type"] = "STRING", ["description"] = "Keywords to search for (e.g., 'gaming laptop', 'iphone 15 pro')." },
                            ["category"] = new JsonObject { ["type"] = "STRING", ["description"] = "The product category (e.g., 'laptop', 'phone')." },
                            ["brand"] = new JsonObject { ["type"] = "STRING", ["description"] = "The product brand (e.g., 'Samsung', 'Apple')." },
                            ["minPrice"] = new JsonObject { ["type"] = "NUMBER", ["description"] = "The minimum price." },
                            ["maxPrice"] = new JsonObject { ["type"] = "NUMBER", ["description"] = "The maximum price." },
                            ["sortBy"] = new JsonObject { ["type"] = "STRING", ["description"] = "The sorting order.", ["enum"] = new JsonArray { "Relevance", "PriceAsc", "PriceDesc", "Newest" } }
                        }
                    }
                },
                new JsonObject
                {
                    ["name"] = "get_product_count",
                    ["description"] = "Gets the total count of products matching a keyword. Useful for inventory questions.",
                    ["parameters"] = new JsonObject
                    {
                        ["type"] = "OBJECT",
                        ["properties"] = new JsonObject
                        {
                            ["keyword"] = new JsonObject { ["type"] = "STRING", ["description"] = "The keyword to count products for." }
                        },
                        ["required"] = new JsonArray { "keyword" }
                    }
                }
            };
        }

        private JsonNode? GetFunctionCall(JsonNode geminiResponse)
        {
            try
            {
                return geminiResponse?["candidates"]?[0]?["content"]?["parts"]?[0]?["functionCall"];
            }
            catch { return null; }
        }

        private string? GetTextContent(JsonNode geminiResponse)
        {
            try
            {
                return geminiResponse?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.GetValue<string>();
            }
            catch { return null; }
        }

        // Helper class for deserializing search criteria from Gemini's function call
        private class ProductSearchCriteria
        {
            public string? keywords { get; set; }
            public string? category { get; set; }
            public string? brand { get; set; }
            public decimal? minPrice { get; set; }
            public decimal? maxPrice { get; set; }
            public ProductSortBy sortBy { get; set; } = ProductSortBy.Relevance;
        }
    }
}