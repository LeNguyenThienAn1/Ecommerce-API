using Application.DTOs;
using EntityHandler.Services.Interface;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
        {
            var response = await _chatService.ProcessUserMessageAsync(request);
            return Ok(response);
        }
    }
}
