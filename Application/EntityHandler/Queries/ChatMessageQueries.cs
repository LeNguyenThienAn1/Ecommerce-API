// Application/EntityHandler/Queries/ChatMessageQueries.cs
using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Infrastructure; // DbContext namespace
using Infrastructure.Entity; // ChatMessageEntity
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.EntityHandler.Queries
{
    public class ChatMessageQueries : IChatMessageQueries
    {
        private readonly EcommerceDbContext _context;

        public ChatMessageQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<ChatMessageDto> GetByIdAsync(Guid id)
        {
            var e = await _context.ChatMessages
                .Include(x => x.Sender)
                .Include(x => x.Receiver)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (e == null) return null;

            return new ChatMessageDto
            {
                Id = e.Id,
                SenderId = e.SenderId,
                ReceiverId = e.ReceiverId,
                Message = e.Message,
                Timestamp = e.Timestamp,
                IsRead = e.IsRead,
                SenderName = e.Sender?.Name,
                ReceiverName = e.Receiver?.Name
            };
        }

        public async Task<IEnumerable<ChatMessageDto>> GetConversationAsync(Guid userA, Guid userB, int page = 1, int pageSize = 50)
        {
            var query = _context.ChatMessages
                .Where(m => (m.SenderId == userA && m.ReceiverId == userB) || (m.SenderId == userB && m.ReceiverId == userA))
                .OrderByDescending(m => m.Timestamp);

            var pageItems = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();

            return pageItems.Select(e => new ChatMessageDto
            {
                Id = e.Id,
                SenderId = e.SenderId,
                ReceiverId = e.ReceiverId,
                Message = e.Message,
                Timestamp = e.Timestamp,
                IsRead = e.IsRead,
                SenderName = e.Sender?.Name,
                ReceiverName = e.Receiver?.Name
            }).OrderBy(m => m.Timestamp); // return chronological order
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsForUserAsync(Guid userId)
        {
            // group by other participant
            var messages = await _context.ChatMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();

            var groups = messages.GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId);

            var result = new List<ConversationDto>();
            foreach (var g in groups)
            {
                var last = g.OrderByDescending(x => x.Timestamp).First();
                var otherUser = last.SenderId == userId ? last.Receiver : last.Sender;
                var unreadCount = g.Count(x => !x.IsRead && x.ReceiverId == userId);

                result.Add(new ConversationDto
                {
                    WithUserId = otherUser.Id,
                    WithUserName = otherUser.Name,
                    LastMessage = new ChatMessageDto
                    {
                        Id = last.Id,
                        SenderId = last.SenderId,
                        ReceiverId = last.ReceiverId,
                        Message = last.Message,
                        Timestamp = last.Timestamp,
                        IsRead = last.IsRead,
                        SenderName = last.Sender?.Name,
                        ReceiverName = last.Receiver?.Name
                    },
                    UnreadCount = unreadCount
                });
            }

            // sort by last message desc
            return result.OrderByDescending(c => c.LastMessage.Timestamp);
        }

        public async Task<int> GetUnreadCountAsync(Guid userId, Guid? fromUserId = null)
        {
            var q = _context.ChatMessages.Where(m => m.ReceiverId == userId && !m.IsRead);
            if (fromUserId.HasValue) q = q.Where(m => m.SenderId == fromUserId.Value);
            return await q.CountAsync();
        }
    }
}
