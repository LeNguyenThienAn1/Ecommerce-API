using Application.EntityHandler.Services;
using Application.EntityHandler.Services.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        // ---------------- CHANGE PASSWORD ----------------
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
        {
            var userIdString = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return Unauthorized("User ID not found in token.");
            }

            var result = await _authService.ChangePasswordAsync(req, userId);
            return Ok(new { message = result });
        }

        // ---------------- ADMIN LOGIN ----------------
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequest req)
        {
            if (req == null)
                return BadRequest("Invalid request.");

            var token = await _authService.LoginAdminAsync(req);
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
