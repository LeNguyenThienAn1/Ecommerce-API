using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => route: /api/products
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lấy tất cả sản phẩm
        /// </summary>
        // GET: /api/products
        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
        {
            var result = await _productService.GetAllProductsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo điều kiện
        /// </summary>
        // POST: /api/products/search
        [HttpPost("search")]
        public async Task<ActionResult<List<ProductInfoDto>>> SearchProducts([FromBody] ProductSearchDto searchDto)
        {
            var result = await _productService.GetAllProductAsync(searchDto);
            return Ok(result);
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        // GET: /api/products/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
        {
            var result = await _productService.GetProductByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật sản phẩm
        /// </summary>
        // POST: /api/products/createupdate
        [HttpPost("createupdate")]
        public async Task<ActionResult<bool>> CreateOrUpdateProduct([FromBody] CreateOrUpdateProductDto dto)
        {
            if (dto == null)
                return BadRequest("Product data is required.");

            var result = await _productService.CreateOrUpdateProductAsync(dto);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating or updating product.");

            return Ok(result);
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        // DELETE: /api/products/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
