using Application.DTOs;
using EntityHandler.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // 💬 Cho phép tất cả người dùng (kể cả chưa đăng nhập)
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            // Truyền Guid.Empty nếu người dùng chưa đăng nhập
            var response = await _chatService.ProcessUserMessageAsync(request, Guid.Empty);
            return Ok(response);
        }
    }
}
