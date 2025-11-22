using System;

namespace Infrastructure.Entity
{
    public class WishlistEntity
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        // Navigation properties
        public UserEntity User { get; set; }
        public ProductEntity Product { get; set; }
    }
}
