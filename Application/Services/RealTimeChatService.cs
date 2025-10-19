using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Application.EntityHandler.Services;
using Infrastructure;
using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Application.Services
{
    public class RealTimeChatService : IRealTimeChatService
    {
        private readonly EcommerceDbContext _context;
        private readonly IChatMessageQueries _queries;

        public RealTimeChatService(EcommerceDbContext context, IChatMessageQueries queries)
        {
            _context = context;
            _queries = queries;
        }

        public async Task<ChatMessageDto> SendMessageAsync(Guid senderId, CreateChatMessageDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (string.IsNullOrWhiteSpace(dto.Message)) throw new ArgumentException("Message cannot be empty");

            var message = new ChatMessageEntity
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Message = dto.Message,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            // Reload to include user nav props (optional)
            await _context.Entry(message).Reference(m => m.Sender).LoadAsync();
            await _context.Entry(message).Reference(m => m.Receiver).LoadAsync();

            return new ChatMessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Message = message.Message,
                Timestamp = message.Timestamp,
                IsRead = message.IsRead,
                SenderName = message.Sender?.Name,
                ReceiverName = message.Receiver?.Name
            };
        }

        public async Task MarkAsReadAsync(Guid ownerUserId, Guid messageId)
        {
            var msg = await _context.ChatMessages.FirstOrDefaultAsync(m => m.Id == messageId && m.ReceiverId == ownerUserId);
            if (msg == null) return; // nothing to do or throw if you prefer
            msg.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public async Task MarkConversationAsReadAsync(Guid ownerUserId, Guid withUserId)
        {
            var items = await _context.ChatMessages
                .Where(m => m.SenderId == withUserId && m.ReceiverId == ownerUserId && !m.IsRead)
                .ToListAsync();

            if (!items.Any()) return;
            items.ForEach(m => m.IsRead = true);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatMessageDto>> GetConversationAsync(Guid userA, Guid userB, int page = 1, int pageSize = 50)
        {
            return await _queries.GetConversationAsync(userA, userB, page, pageSize);
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsForUserAsync(Guid userId)
        {
            return await _queries.GetConversationsForUserAsync(userId);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId, Guid? fromUserId = null)
        {
            return await _queries.GetUnreadCountAsync(userId, fromUserId);
        }
    }
}
