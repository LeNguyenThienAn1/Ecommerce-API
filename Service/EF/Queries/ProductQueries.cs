using Core.Queries;
using Entity;
using Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.Queries
{
    public class ProductQueries : IProductQueries
    {
        public async Task<ProductEntity> GetProductAsync()
        {
            return new ProductEntity()
            {
                Id = Guid.NewGuid(),
                Name = "Iphone15",
                Description = "Description",
                Brand = ProductBrand.Apple,
                Category = ProductCategory.Phone,
                Price = 100,
                Stock = 10,
                ImageUrl = "",
                IsFeatured = true,
                IsOnSale = true,
            };
        }
    }
}
