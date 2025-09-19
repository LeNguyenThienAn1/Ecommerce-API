using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => /api/admin
    public class AdminController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public AdminController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        // ================== CATEGORY ===================
        /// <summary>
        /// Lấy tất cả danh mục
        /// </summary>
        // GET: /api/admin/categories
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryInfoDto>>> GetCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync(null);
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm danh mục theo filter
        /// </summary>
        // POST: /api/admin/categories/search
        [HttpPost("categories/search")]
        public async Task<ActionResult<List<CategoryInfoDto>>> SearchCategories([FromBody] CategoryFilterDto filterDto)
        {
            var result = await _categoryService.GetAllCategoriesAsync(filterDto);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        // GET: /api/admin/categories/{id}
        [HttpGet("categories/{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật danh mục
        /// </summary>
        // POST: /api/admin/categories/createupdate
        [HttpPost("categories/createupdate")]
        public async Task<ActionResult<bool>> CreateOrUpdateCategory([FromBody] CreateOrUpdateCategoryDto dto)
        {
            if (dto == null) return BadRequest("Category data is required.");

            var result = await _categoryService.CreateOrUpdateCategoryAsync(dto);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating or updating category.");

            return Ok(result);
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        // DELETE: /api/admin/categories/{id}
        [HttpDelete("categories/{id}")]
        public async Task<ActionResult<bool>> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }

        // ================== PRODUCT ===================
        /// <summary>
        /// Lấy tất cả sản phẩm trong admin (có phân trang, lọc, sort)
        /// </summary>
        // GET: /api/admin/products
        [HttpPost("Paging")]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetPagedProducts([FromBody] ProductPagingRequestDto request)
        {
            var result = await _productService.GetPagedProductsAsync(request);
            return Ok(result);
        }


        /// <summary>
        /// Tìm kiếm sản phẩm theo filter
        /// </summary>
        // POST: /api/admin/products/search
        //[HttpPost("products/search")]
        //public async Task<ActionResult<List<ProductInfoDto>>> SearchProducts([FromBody] ProductSearchDto dto)
        //{
        //    var result = await _productService.GetAllProductAsync(dto);
        //    return Ok(result);
        //}

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        // GET: /api/admin/products/{id}
        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật sản phẩm
        /// </summary>
        // POST: /api/admin/products/createupdate
        [HttpPost("products/createupdate")]
        public async Task<ActionResult<bool>> CreateOrUpdateProduct([FromBody] CreateOrUpdateProductDto dto)
        {
            if (dto == null) return BadRequest("Product data is required.");

            var result = await _productService.CreateOrUpdateProductAsync(dto);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating or updating product.");

            return Ok(result);
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        // DELETE: /api/admin/products/{id}
        [HttpDelete("products/{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
