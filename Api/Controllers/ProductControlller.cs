using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => route: /api/products
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // POST: /api/products/get
        [HttpPost("get")]
        public async Task<ActionResult<List<ProductDto>>> GetProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        // POST: /api/products/search
        [HttpPost("search")]
        public async Task<ActionResult<List<ProductInfoDto>>> SearchProducts([FromBody] ProductSearchDto searchDto)
        {
            var result = await _productService.GetAllProductAsync(searchDto);
            return Ok(result);
        }

        // GET: /api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }
    }
}
