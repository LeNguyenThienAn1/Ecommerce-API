using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
        public string? CreateBy { get; set; } = string.Empty;
        public string? UpdateBy { get; set; } = string.Empty;
    }
}
