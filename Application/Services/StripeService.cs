using Stripe;
using Stripe.Checkout;
using Application.DTOs;
using Application.EntityHandler.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.EntityHandler.Services.Implementations
{
    public class StripeService : IStripeService
    {
        private readonly IConfiguration _config;

        public StripeService(IConfiguration config)
        {
            _config = config;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<string> CreateCheckoutSessionAsync(StripePaymentDto dto)
        {
            try
            {
                // 1️⃣ Kiểm tra dữ liệu đầu vào
                if (dto.Amount <= 0)
                    throw new Exception("Amount must be greater than 0.");
                if (string.IsNullOrWhiteSpace(dto.Currency))
                    throw new Exception("Currency is required (vd: 'usd').");
                if (string.IsNullOrWhiteSpace(dto.SuccessUrl) || string.IsNullOrWhiteSpace(dto.CancelUrl))
                    throw new Exception("SuccessUrl và CancelUrl là bắt buộc.");

                // 2️⃣ Chuẩn hóa
                dto.Currency = dto.Currency.Trim().ToLower();
                var supportedCurrencies = new HashSet<string>
                {
                    "usd", "vnd", "eur", "gbp", "jpy", "sgd", "aud", "cad"
                };
                if (!supportedCurrencies.Contains(dto.Currency))
                    throw new Exception($"Unsupported currency: {dto.Currency}.");

                // 3️⃣ Không thêm https:// vì Stripe cho phép localhost trong test mode
                var successUrl = dto.SuccessUrl.Trim();
                var cancelUrl = dto.CancelUrl.Trim();

                Console.WriteLine("──────────────────────────────");
                Console.WriteLine($"[Stripe] ✅ Success URL: {successUrl}");
                Console.WriteLine($"[Stripe] ✅ Cancel URL: {cancelUrl}");
                Console.WriteLine($"[Stripe] ✅ Amount: {dto.Amount}");
                Console.WriteLine($"[Stripe] ✅ Currency: {dto.Currency}");
                Console.WriteLine($"[Stripe] ✅ OrderId: {dto.OrderId}");
                Console.WriteLine("──────────────────────────────");

                // 4️⃣ Cấu hình phiên Checkout
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new()
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = dto.Currency,
                                // Stripe yêu cầu đơn vị nhỏ nhất (cents)
                                UnitAmount = dto.Amount,
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Order #{dto.OrderId}"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    SuccessUrl = $"{successUrl}?orderId={dto.OrderId}&sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = cancelUrl

                };

                // 5️⃣ Tạo session Stripe
                var service = new SessionService();
                var session = await service.CreateAsync(options);

                Console.WriteLine($"[Stripe] ✅ Session created: {session.Id}");
                Console.WriteLine($"[Stripe] 🌐 Checkout URL: {session.Url}");

                return session.Url ?? throw new Exception("Stripe session URL is null.");
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"❌ [Stripe API Error] {ex.Message}");
                throw new Exception($"Stripe API error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [StripeService Error] {ex.Message}");
                throw;
            }
        }

        public async Task<bool> ConfirmPaymentAsync(string sessionId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sessionId))
                    throw new Exception("Session ID là bắt buộc.");

                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                Console.WriteLine($"[Stripe] ✅ Payment status for {sessionId}: {session.PaymentStatus}");

                return session.PaymentStatus == "paid";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [Stripe ConfirmPayment Error] {ex.Message}");
                throw;
            }
        }
    }
}
