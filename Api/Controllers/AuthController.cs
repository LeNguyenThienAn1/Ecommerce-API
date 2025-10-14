using Application.EntityHandler.Services;
using Application.EntityHandler.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.AuthDto;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ---------------- REGISTER ----------------
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            if (req == null)
                return BadRequest("Invalid request.");

            var result = await _authService.RegisterAsync(req);
            return Ok(new { message = result });
        }

        // ---------------- LOGIN ----------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (req == null)
                return BadRequest("Invalid request.");

            var token = await _authService.LoginAsync(req);
            return Ok(token);
        }

        // ---------------- REFRESH TOKEN ----------------
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest req)
        {
            if (string.IsNullOrEmpty(req.RefreshToken))
                return BadRequest("Missing refresh token.");

            var newTokens = await _authService.RefreshTokenAsync(req);
            return Ok(newTokens);
        }

        // ---------------- LOGOUT ----------------
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest req)
        {
            if (string.IsNullOrEmpty(req.RefreshToken))
                return BadRequest("Missing refresh token.");

            await _authService.LogoutAsync(req);
            return Ok(new { message = "Logout successful" });
        }
    }
}
