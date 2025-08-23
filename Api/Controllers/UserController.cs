//using Application.DTOs;
//using Microsoft.AspNetCore.Mvc;

//namespace Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")] // => route sẽ là: /api/users
//    public class UsersController : ControllerBase
//    {
//        private readonly UserService _userService;

//        public UsersController(UserService userService)
//        {
//            _userService = userService;
//        }

//        // POST: /api/users/get
//        [HttpPost("get")]
//        public async Task<ActionResult<UserDto>> GetUser()
//        {
//            var result = await _userService.GetUserAsync();
//            return Ok(result);
//        }
//    }
//}
