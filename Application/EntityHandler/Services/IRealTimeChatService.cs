using Application.DTOs;
using System.Threading.Tasks;

namespace Application.EntityHandler.Services
{
    public interface IRealTimeChatService
    {
        Task<ChatMessageDto> SendMessageAsync(Guid senderId, CreateChatMessageDto dto);
        Task MarkAsReadAsync(Guid ownerUserId, Guid messageId);
        Task MarkConversationAsReadAsync(Guid ownerUserId, Guid withUserId);
        Task<IEnumerable<ChatMessageDto>> GetConversationAsync(Guid userA, Guid userB, int page = 1, int pageSize = 50);
        Task<IEnumerable<ConversationDto>> GetConversationsForUserAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId, Guid? fromUserId = null);
    }
}
