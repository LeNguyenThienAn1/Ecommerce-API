using Application.DTOs;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMomoService
    {
        Task<MomoPaymentResponseDto?> CreatePaymentAsync(OrderDto order, decimal totalAmount);
        Task<bool> ValidateSignatureAsync(MomoIPNResponseDto ipnDto);
    }
}
