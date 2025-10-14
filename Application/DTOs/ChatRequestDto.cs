using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ChatRequestDto
    {
        public int UserId { get; set; }   // optional, nếu có login
        public string Message { get; set; }
    }
}
