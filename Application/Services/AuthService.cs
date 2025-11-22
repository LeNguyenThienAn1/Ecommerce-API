using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Infrastructure;
using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static Application.DTOs.AuthDto;

namespace Application.EntityHandler.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly EcommerceDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(EcommerceDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // ✅ Chuẩn hóa số điện thoại (đảm bảo backend luôn thống nhất format)
        private string NormalizePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return phone;

            phone = phone.Trim();

            if (phone.StartsWith("0"))
                return "+84" + phone.Substring(1);
            if (phone.StartsWith("84") && !phone.StartsWith("+84"))
                return "+" + phone;

            return phone;
        }

        // --------------------- ĐĂNG KÝ ---------------------
        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            request.PhoneNumber = NormalizePhone(request.PhoneNumber);

            if (await _db.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber))
                throw new Exception("Số điện thoại đã được đăng ký.");

            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                PhoneNumber = request.PhoneNumber,
                Name = request.Name ?? "Người dùng mới",
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = UserType.Customer,
                CreateAt = DateTime.UtcNow
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            return "Đăng ký thành công.";
        }

        // --------------------- ĐĂNG NHẬP ---------------------
        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            request.PhoneNumber = NormalizePhone(request.PhoneNumber);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
                throw new Exception("Tài khoản không tồn tại.");

            // ⚠️ Kiểm tra nếu tài khoản bị khóa
            if (!user.IsActive)
                throw new Exception("Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ hỗ trợ.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new Exception("Sai mật khẩu.");

            var tokens = GenerateJwtTokens(user);

            user.RefreshToken = tokens.RefreshToken;
            await _db.SaveChangesAsync();

            return tokens;
        }


        // --------------------- ĐĂNG NHẬP ADMIN ---------------------
        public async Task<AuthResult> LoginAdminAsync(LoginRequest request)
        {
            request.PhoneNumber = NormalizePhone(request.PhoneNumber);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);
            if (user == null)
                throw new Exception("Tài khoản không tồn tại.");

            if (user.Role != UserType.Admin)
                throw new Exception("Tài khoản không có quyền truy cập.");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new Exception("Sai mật khẩu.");

            var tokens = GenerateJwtTokens(user);

            user.RefreshToken = tokens.RefreshToken;
            await _db.SaveChangesAsync();

            return tokens;
        }

        // --------------------- LÀM MỚI TOKEN ---------------------
        public async Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            if (user == null)
                throw new Exception("Refresh token không hợp lệ.");

            var tokens = GenerateJwtTokens(user);

            user.RefreshToken = tokens.RefreshToken;
            await _db.SaveChangesAsync();

            return tokens;
        }

        // --------------------- ĐĂNG XUẤT ---------------------
        public async Task LogoutAsync(LogoutRequest request)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            if (user != null)
            {
                user.RefreshToken = null;
                await _db.SaveChangesAsync();
            }
        }

        // --------------------- HÀM SINH TOKEN ---------------------
        private AuthResult GenerateJwtTokens(UserEntity user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(30);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim("phone", user.PhoneNumber ?? string.Empty),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new AuthResult
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = expires,
                User = new UserProfileDto
                {
                    Id = user.Id,
                    PhoneNumber = user.PhoneNumber,
                    Name = user.Name,
                    Role = user.Role.ToString()
                }
            };
        }
    }
}
