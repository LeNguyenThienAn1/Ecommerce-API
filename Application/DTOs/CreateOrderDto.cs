using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateOrderDto
    {
        public Guid BoughtBy { get; set; }  // Id của user tạo order
        public List<Guid> ProductIds { get; set; } = new(); // Danh sách sản phẩm trong đơn hàng
    }
}
