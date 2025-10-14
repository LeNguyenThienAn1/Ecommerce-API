using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ChatResponseDto
    {
        public string BotMessage { get; set; }
        public List<ProductDto> Products { get; set; } = new List<ProductDto>();
    }
}
