using Application.DTOs;

namespace Application.EntityHandler.Services
{
    public interface IStripeService
    {
        Task<string> CreateCheckoutSessionAsync(StripePaymentDto dto);
        Task<bool> ConfirmPaymentAsync(string sessionId);
    }
}
