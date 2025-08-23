using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
        public class BaseDto
        {
            public Guid Id { get; set; }
            public DateTime CreateAt { get; set; }
            public DateTime UpdateAt { get; set; } = DateTime.Now;
            public string CreateBy { get; set; }
            public string UpdateBy { get; set; }
        }
    }
