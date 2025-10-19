// Application/EntityHandler/Queries/Interface/IChatMessageQueries.cs
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.EntityHandler.Queries.Interface
{
    public interface IChatMessageQueries
    {
        Task<IEnumerable<ChatMessageDto>> GetConversationAsync(Guid userA, Guid userB, int page = 1, int pageSize = 50);
        Task<IEnumerable<ConversationDto>> GetConversationsForUserAsync(Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId, Guid? fromUserId = null);
        Task<ChatMessageDto> GetByIdAsync(Guid id);
    }
}
