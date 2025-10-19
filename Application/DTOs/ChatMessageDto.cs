// Application/DTOs/ChatDto.cs
using System;

namespace Application.DTOs
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
        public string SenderName { get; set; } // optional
        public string ReceiverName { get; set; } // optional
    }

    public class CreateChatMessageDto
    {
        public Guid ReceiverId { get; set; }
        public string Message { get; set; }
    }

    public class ConversationDto
    {
        public Guid WithUserId { get; set; }
        public string WithUserName { get; set; }
        public ChatMessageDto LastMessage { get; set; }
        public int UnreadCount { get; set; }
    }
}
