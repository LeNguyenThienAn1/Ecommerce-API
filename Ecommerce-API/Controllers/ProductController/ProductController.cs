using Ecommerce_API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce_API
{
    public class ProductController
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("getproductbyid/{id}")]
        public ProductDetailsDto GetProductInformationByIdAsync(Guid id)
        {
            var productDetails =  _productService.GetProductInformationByIdAsync(id);
            if (productDetails == null)
            {
                return null;
            }
            return productDetails;
        }
        [HttpPost("getproductspaging)]
        public List<ProductDetailsDto> GetProductsPaging([FromBody] PagingRequestDto pagingRequest)
        {
            var products = _productService.GetProductsPaging(pagingRequest);
            if (products == null || !products.Any())
            {
                return new List<ProductDetailsDto>();
            }
            return products;
        }
    }
}
