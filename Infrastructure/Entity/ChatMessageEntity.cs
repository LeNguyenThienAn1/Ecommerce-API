
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entity
{
    public class ChatMessageEntity : BaseEntity
    {
        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        [ForeignKey("SenderId")]
        public virtual UserEntity Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual UserEntity Receiver { get; set; }
    }
}
