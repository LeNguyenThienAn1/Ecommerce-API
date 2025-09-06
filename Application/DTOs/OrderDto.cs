using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class OrderDto : BaseDto
    {
    }
    public class CreateOrderDto
    {
        public List<Guid> ProductIds { get; set; }
        public Guid BoughtBy { get; set; }
    }
}
