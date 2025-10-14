using Application.DTOs;
using Application.EntityHandler.Services;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => /api/products
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;

        public ProductsController(
            IProductService productService,
            IOrderService orderService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _orderService = orderService;
            _categoryService = categoryService;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm (có phân trang, lọc, sort)
        /// </summary>
        // POST: /api/products/paging
        [HttpPost("paging")]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetPagedProducts([FromBody] ProductPagingRequestDto request)
        {
            if (request == null)
                return BadRequest("Request không hợp lệ.");

            var result = await _productService.GetPagedProductsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Lấy tất cả thương hiệu (Brand)
        /// </summary>
        // GET: /api/products/brands
        [HttpGet("brands")]
        public async Task<ActionResult<List<BrandDto>>> GetAllBrands()
        {
            var result = await _productService.GetAllBrandsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy tất cả danh mục (Category)
        /// </summary>
        // GET: /api/products/categories
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryInfoDto>>> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
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
                return NotFound(new { message = "Không tìm thấy sản phẩm." });

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
                return BadRequest("Dữ liệu sản phẩm không hợp lệ.");

            var result = await _productService.CreateOrUpdateProductAsync(dto);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi tạo hoặc cập nhật sản phẩm.");

            return Ok(new { success = true });
        }

        /// <summary>
        /// Xóa sản phẩm theo ID
        /// </summary>
        // DELETE: /api/products/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
                return NotFound(new { message = "Không tìm thấy sản phẩm để xóa." });

            return Ok(new { success = true });
        }

        /// <summary>
        /// Checkout: tạo đơn hàng từ giỏ hàng
        /// </summary>
        // POST: /api/products/checkout
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CreateOrderRequest request)
        {
            if (request == null || request.Order?.ProductIds == null || !request.Order.ProductIds.Any())
                return BadRequest("Giỏ hàng trống hoặc dữ liệu không hợp lệ.");

            var result = await _orderService.CreateOrderAsync(request.Order, request.Bill);

            if (result != Guid.Empty)
                return Ok(new { success = true, message = "Đặt hàng thành công", OrderId = result });

            return StatusCode(500, new { success = false, message = "Có lỗi xảy ra khi tạo đơn hàng" });
        }
    }
}
