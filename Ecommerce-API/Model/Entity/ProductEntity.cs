using System.Collections.Generic;
using static Ecommerce_API.Model.Enum.EcommerceEnum;

namespace Ecommerce_API.Model.Entity
{
    public class ProductEntity : CommonEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public Brand Brand { get; set; }
        public ProductCategory Category { get; set; }
        public decimal? Discount { get; set; }
        public List<string> ImageUrl { get; set; } = new List<string>();
        public bool IsTrending { get; set; } = false;
        public int StockQuantity { get; set; }
        public string SKU { get; set; }
        public string ModelNumber { get; set; }
        public string WarrantyInformation { get; set; }
        public string Specifications { get; set; } 
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public virtual ICollection<ReviewEntity> Reviews { get; set; }

        public ProductEntity()
        {
            Reviews = new HashSet<ReviewEntity>();
        }
    }
}
