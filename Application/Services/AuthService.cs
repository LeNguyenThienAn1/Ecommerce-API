//using Application.DTOs;
//using Application.EntityHandler.Services;
//using Application.Interfaces.Queries;
//using Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace Application.Services
//{
//    public class AuthService : IAuthService
//    {
//        private readonly EcommerceDbContext _context;
//        private readonly IUserQueries _userQueries;
//        private readonly string _jwtSecret = "your_secret_key_here"; // TODO: lấy từ appsettings.json

//        public AuthService(EcommerceDbContext context, IUserQueries userQueries)
//        {
//            _context = context;
//            _userQueries = userQueries;
//        }

//        public async Task<AuthenticateResponseDto?> Login(string phoneNumber)
//        {
//            try
//            {
//                var user = await _userQueries.GetUserByPhoneNumber(phoneNumber);

//                if (user == null) return null; // không tồn tại số điện thoại

//                var jwtToken = GenerateAccessToken(user);
//                var refreshToken = GenerateRefreshToken(user);

//                user.RefreshToken = refreshToken;
//                _context.Users.Update(user);
//                await _context.SaveChangesAsync();

//                return new AuthenticateResponseDto(user, jwtToken, refreshToken);
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine(ex);
//                return null;
//            }
//        }

//        public async Task<AuthenticateResponseDto?> RefreshToken(string token)
//        {
//            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == token);
//            if (user == null) return null;

//            var refreshToken = ValidateToken(token);
//            if (refreshToken == null) return null;

//            var userIdFromToken = refreshToken.Claims
//                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

//            if (userIdFromToken == null || user.Id != long.Parse(userIdFromToken))
//                return null;

//            var jwtToken = GenerateAccessToken(user);
//            return new AuthenticateResponseDto(user, jwtToken, null);
//        }

//        public async Task RevokeToken(string refreshToken)
//        {
//            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);
//            if (user == null) return;

//            user.RefreshToken = null;
//            _context.Users.Update(user);
//            await _context.SaveChangesAsync();
//        }

//        private string GenerateAccessToken(UserEntity user)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_jwtSecret);

//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new Claim[]
//                {
//                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                    new Claim(ClaimTypes.Role, user.Role.ToString())
//                }),
//                Expires = DateTime.UtcNow.AddMinutes(30),
//                SigningCredentials = new SigningCredentials(
//                    new SymmetricSecurityKey(key),
//                    SecurityAlgorithms.HmacSha256Signature
//                )
//            };

//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }

//        private string GenerateRefreshToken(UserEntity user)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_jwtSecret);

//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new Claim[]
//                {
//                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//                }),
//                Expires = DateTime.UtcNow.AddDays(7),
//                SigningCredentials = new SigningCredentials(
//                    new SymmetricSecurityKey(key),
//                    SecurityAlgorithms.HmacSha256Signature
//                )
//            };

//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }

//        private JwtSecurityToken? ValidateToken(string token)
//        {
//            try
//            {
//                var tokenHandler = new JwtSecurityTokenHandler();
//                var key = Encoding.ASCII.GetBytes(_jwtSecret);

//                tokenHandler.ValidateToken(token, new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(key),
//                    ValidateIssuer = false,
//                    ValidateAudience = false,
//                }, out SecurityToken validatedToken);

//                return (JwtSecurityToken)validatedToken;
//            }
//            catch (Exception e)
//            {
//                Console.Error.WriteLine(e);
//                return null;
//            }
//        }
//    }
//}
