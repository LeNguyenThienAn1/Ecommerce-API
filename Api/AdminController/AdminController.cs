using Application.DTOs;
using Application.Interfaces.Services;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Api.AdminController
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public AdminController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        // ================= CATEGORY =================
        /// <summary>
        /// Lấy tất cả danh mục (có thể filter theo Id)
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryInfoDto>>> GetAllCategories([FromQuery] CategoryFilterDto filter)
        {
            var categories = await _categoryService.GetAllCategoriesAsync(filter);
            return Ok(categories);
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        [HttpGet("category/{id}")]
        public async Task<ActionResult<CategoryEntity>> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }
        /// <summary>
        /// Tạo mới hoặc cập nhật danh mục
        /// </summary>
        [HttpPost("createupdatecategory")]
        public async Task<ActionResult<bool>> CreateOrUpdateCategory([FromBody] CreateOrUpdateCategoryDto dto)
        {
            if (dto == null)
                return BadRequest("Category data is required.");

            var result = await _categoryService.CreateOrUpdateCategoryAsync(dto);

            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating or updating category.");

            return Ok(result);
        }

        // ================= PRODUCT =================

        /// <summary>
        /// Lấy tất cả sản phẩm
        /// </summary>
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Lấy sản phẩm theo ID
        /// </summary>
        [HttpGet("product/{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật sản phẩm
        /// </summary>
        [HttpPost("createupdateproduct")]
        public async Task<ActionResult<bool>> CreateOrUpdateProduct([FromBody] CreateOrUpdateProductDto dto)
        {
            if (dto == null)
                return BadRequest("Product data is required.");

            var result = await _productService.CreateOrUpdateProductAsync(dto);

            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating or updating product.");

            return Ok(result);
        }
        [HttpDelete("category/{id}")]
        public async Task<ActionResult<bool>> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary>
        [HttpDelete("product/{id}")]
        public async Task<ActionResult<bool>> DeleteProduct(Guid id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
