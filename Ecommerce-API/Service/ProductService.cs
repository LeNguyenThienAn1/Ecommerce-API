using Ecommerce_API.DTOs;
using Ecommerce_API.IQueries;
using Ecommerce_API.Model.Entity;

namespace Ecommerce_API
{
    public class ProductService : IProductService
    {
        private readonly IProductQuery _productQuery;

        public ProductService(IProductQuery productQuery)
        {
            _productQuery = productQuery;
        }

        public ProductDetailsDto GetProductInformationByIdAsync(Guid id)
        {
            var productEntity = _productQuery.GetProductInfomrationById(id);
            var result = new ProductDetailsDto
            {
                Name = productEntity.Name,
                Price = productEntity.Price,
                Description = productEntity.Description
            };
            return result;
        }

    }
}
