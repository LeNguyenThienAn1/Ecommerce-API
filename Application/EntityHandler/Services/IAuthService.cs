using Application.DTOs;
using static Application.DTOs.AuthDto;

namespace Application.EntityHandler.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequest request);
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request);
        Task LogoutAsync(LogoutRequest request);
    }
}
