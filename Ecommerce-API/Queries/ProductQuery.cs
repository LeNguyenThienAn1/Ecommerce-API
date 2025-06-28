using Ecommerce_API.IQueries;
using Ecommerce_API.Model.Entity;
using static Ecommerce_API.EcommerceEnum;

namespace Ecommerce_APi
{
    public class ProductQuery : IProductQuery
    {
        public ProductEntity GetProductInfomrationById(Guid id)
        {
            try
            {
                var productEntity = new ProductEntity
                {
                    Id = Guid.NewGuid(),
                    Name = "Iphone 14",
                    Description = "",
                    Price = 19.99m,
                    Brand = Brand.Apple,
                    Category = ProductCategory.Phone,
                };
                return  productEntity;
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                throw new Exception("An error occurred while fetching product information.", ex);
            }
        }
    }
}
