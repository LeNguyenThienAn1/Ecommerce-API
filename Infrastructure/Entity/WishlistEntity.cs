using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entity
{
    public class WishlistEntity : BaseEntity
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }

        // Quan hệ
        public UserEntity User { get; set; }
        public ProductEntity Product { get; set; }
    }
}
