using System;

namespace Application.DTOs
{
    public class WishlistDto : BaseDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ProductDescription { get; set; } = string.Empty;
        public Guid? BrandId { get; set; }
        public string? BrandName { get; set; }

        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }

    }
}
