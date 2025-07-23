using Core.Queries;
using Entity;
using Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiService.Impl
{
    public class ProductService : IProductService
    {
        private readonly IProductQueries _productQueries;
        public ProductService(IProductQueries productQueries)
        {
            _productQueries = productQueries;
        }

        public async Task<ProductDto> GetProductAsync()
        {
            var productEntity = await _productQueries.GetProductAsync();
            var result = new ProductDto()
            {
                Name = productEntity.Name,
                Category = productEntity.Category,
                Brand = productEntity.Brand,
                Price = productEntity.Price,
            };
            return result;
        }
    }
}
