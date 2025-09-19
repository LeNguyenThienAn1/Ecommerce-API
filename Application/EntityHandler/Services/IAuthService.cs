using System.Threading.Tasks;
using Application.DTOs;
using Infrastructure; // để dùng UserEntity

namespace Application.EntityHandler.Services
{
    public interface IAuthService
    {
        Task<AuthenticateResponseDto?> Login(string phoneNumber);
        Task<AuthenticateResponseDto?> RefreshToken(string refreshToken);
        Task RevokeToken(string refreshToken);
        Task<UserEntity?> GetUserByPhoneNumber(string phoneNumber);
    }
}
