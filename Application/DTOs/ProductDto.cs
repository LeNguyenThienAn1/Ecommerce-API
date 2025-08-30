using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class ProductDto : BaseDto
    {
        //public string Name { get; set; }
        //public string Description { get; set; }
        //public decimal Price { get; set; }
        //public string ImageUrl { get; set; }
        //blic int Stock { get; set; }
        //public bool IsFeatured { get; set; }
        //public ProductFeaturedType FeaturedType { get; set; }
        //public int? SalePercent { get; set; }
        //public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; } = 1;
        public bool IsFeatured { get; set; }
        public ProductFeaturedType FeaturedType { get; set; }
        public int? SalePercent { get; set; }
        //public Guid CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public ProductBrand Brand { get; set; }
        public ProductDetail Detail { get; set; }

    }

    // DTO dùng để nhận dữ liệu từ FE khi Create/Update
    public class CreateOrUpdateProductDto
    {
        public Guid Id { get; set; }  // khi tạo mới => Guid.Empty, khi update => truyền Id
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Stock { get; set; } = 1;
        public bool IsFeatured { get; set; }
        public ProductFeaturedType FeaturedType { get; set; }
        public int? SalePercent { get; set; }
        //public Guid CategoryId { get; set; }
        public ProductCategory Category { get; set; }
        public ProductBrand Brand { get; set; }
        public ProductDetail Detail { get; set; }
    }
    public class ProductSearchDto
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }

    }
    public class ProductInfoDto
    {
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
