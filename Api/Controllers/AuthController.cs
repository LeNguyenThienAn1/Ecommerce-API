//using Application.DTOs;
//using Api.Controllers.Contract;
//using Application.DTOS;
//using Api.Service.Database;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using NuGet.Common;

//namespace Api.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AuthController : IRentController
//    {
//        public AuthController(IUnitOfWork serviceWrapper) : base(serviceWrapper)
//        {
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login(LoginInfo loginInfo)
//        {
//            if (loginInfo != null)
//            {
//                var response = await Service.AuthService.Login(loginInfo.Email, loginInfo.Password);

//                if (response == null)
//                    return BadRequest(new { message = "Username or password is incorrect" });

//                SetTokenCookie(response.RefreshToken);

//                return Ok(response);
//            }
//            else return BadRequest();
//        }

//        [AllowAnonymous]
//        [HttpPost("refresh-token")]
//        public async Task<IActionResult> RefreshToken()
//        {
//            var refreshToken = Request.Cookies["refreshToken"];
//            if (refreshToken != null)
//            {
//                var response = await Service.AuthService.RefreshToken(refreshToken);

//                if (response == null) return BadRequest("Invalid refresh token");

//                return Ok(response);
//            }
//            else
//            {
//                return BadRequest("Invalid refresh token");
//            }
//        }

//        [HttpPost("revoke-token")]
//        public async Task<IActionResult> RevokeToken()
//        {
//            var refreshToken = Request.Cookies["refreshToken"];
//            if (refreshToken != null)
//            {
//                await Service.AuthService.RevokeToken(refreshToken);

//                RemoveTokenCookie();

//                return NoContent();
//            }
//            else
//            {
//                return BadRequest("Invalid refresh token");
//            }
//        }

//        private void RemoveTokenCookie()
//        {
//            var cookieOptions = new CookieOptions
//            {
//                HttpOnly = true,
//                Expires = DateTime.UtcNow.AddDays(-1),
//                SameSite = SameSiteMode.None,
//                Secure = true
//            };
//            Response.Cookies.Append("refreshToken", "", cookieOptions);
//        }

//        private void SetTokenCookie(string token)
//        {
//            var cookieOptions = new CookieOptions
//            {
//                HttpOnly = true,
//                Expires = DateTime.UtcNow.AddYears(100),
//                SameSite = SameSiteMode.None,
//                Secure = true
//            };
//            Response.Cookies.Append("refreshToken", token, cookieOptions);
//        }
//    }
//}
