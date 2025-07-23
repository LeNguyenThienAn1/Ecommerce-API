using ApiService.Impl;
using Microsoft.AspNetCore.Mvc;
using Model.DTOs;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => route sẽ là: /api/products
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // POST: /api/products/get
        [HttpPost("get")]
        public async Task<ActionResult<ProductDto>> GetProduct()
        {
            var result = await _productService.GetProductAsync();
            return Ok(result);
        }
    }
}
