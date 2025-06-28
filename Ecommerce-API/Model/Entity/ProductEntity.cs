using static Ecommerce_API.EcommerceEnum;
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
        //public int Discount { get; set; }
        public string ImageUrl { get; set; }
        public bool IsTrending { get; set; } = false;
    }
}
