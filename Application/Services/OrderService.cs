using Application.DTOs;
using Application.Interfaces.Queries;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IProductQueries _productQueries;

        public OrderService(IProductQueries productQueries)
        {
            _productQueries = productQueries;
        }
        public async Task<bool> CreateOrderAsync(CreateOrderDto request)
        {
            foreach (var productId in request.ProductIds)
            {
                var productEntity = await _productQueries.GetProductByIdAsync(productId);
                productEntity.Status = ProductStatus.Sold;
                productEntity.BoughtBy = request.BoughtBy;
                await _productQueries.UpdateProductAsync(productEntity);
            }
            return true;
        }
    }
}
