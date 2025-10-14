using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class OrderEntity : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CustomerName { get; set; }
        public string ShippingAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string PaymentMethod { get; set; }
        public string Note { get; set; }
        public OrderStatus Status { get; set; }
        public Guid UserId { get; set; }
        public ICollection<OrderDetailEntity> Details { get; set; }
    }


    public class OrderDetailEntity
    {
        public Guid OrderId { get; set; }
        public OrderEntity Order { get; set; }

        public Guid ProductId { get; set; }
        public ProductEntity Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
