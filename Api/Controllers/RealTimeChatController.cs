// Api/Controllers/ChatController.cs
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.EntityHandler.Services;

namespace Api.Controllers
{
   [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RealTimeChatController : ControllerBase
    {
        private readonly IRealTimeChatService _chatService;
        private readonly IHubContext<ChatHub> _hub;

        public RealTimeChatController(IRealTimeChatService chatService, IHubContext<ChatHub> hub)
        {
            _chatService = chatService;
            _hub = hub;
        }

        private Guid GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("id")
                          ?? User.FindFirst("sub");

            if (idClaim == null)
            {
                throw new UnauthorizedAccessException("Không tìm th?y claim ID trong token.");
            }

            return Guid.Parse(idClaim.Value);
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = GetUserId();
            var conv = await _chatService.GetConversationsForUserAsync(userId);
            return Ok(conv);
        }

        [HttpGet("conversation/{withUserId}")]
        public async Task<IActionResult> GetConversation(Guid withUserId, int page = 1, int pageSize = 50)
        {
            var userId = GetUserId();
            var messages = await _chatService.GetConversationAsync(userId, withUserId, page, pageSize);
            return Ok(messages);
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] CreateChatMessageDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest(new { error = "Request body is null." });

                if (dto.ReceiverId == Guid.Empty)
                    return BadRequest(new { error = "ReceiverId cannot be empty." });

                var senderId = GetUserId();

                // G?i service ?ã check receiver t?n t?i
                var result = await _chatService.SendMessageAsync(senderId, dto);

                // push realtime to receiver via SignalR group
                await _hub.Clients.Group($"user-{dto.ReceiverId}")
                    .SendAsync("ReceiveMessage", result);

                // optionally notify sender too
                await _hub.Clients.Group($"user-{senderId}")
                    .SendAsync("MessageSent", result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Tr? v? l?i rõ ràng, không crash server
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpPost("mark-read/{messageId}")]
        public async Task<IActionResult> MarkAsRead(Guid messageId)
        {
            var userId = GetUserId();
            await _chatService.MarkAsReadAsync(userId, messageId);
            return NoContent();
        }

        [HttpPost("mark-conversation-read/{withUserId}")]
        public async Task<IActionResult> MarkConversationRead(Guid withUserId)
        {
            var userId = GetUserId();
            await _chatService.MarkConversationAsReadAsync(userId, withUserId);
            return NoContent();
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnread([FromQuery] Guid? fromUser)
        {
            var userId = GetUserId();
            var count = await _chatService.GetUnreadCountAsync(userId, fromUser);
            return Ok(new { unread = count });
        }
    }
}
