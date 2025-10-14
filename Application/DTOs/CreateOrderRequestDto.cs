using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateOrderRequest
    {
        public CreateOrderDto Order { get; set; }
        public BillInfoDto Bill { get; set; }
    }
}
