using Application.DTOs;
using Application.EntityHandler.Services;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IBrandService _brandService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;

        public AdminController(
            ICategoryService categoryService,
            IProductService productService,
            IBrandService brandService,
            IOrderService orderService,
            IUserService userService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _brandService = brandService;
            _orderService = orderService;
            _userService = userService;
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
        // POST: /api/admin/categories/filter
        [HttpPost("categories/filter")]
        public async Task<ActionResult<List<CategoryInfoDto>>> FilterCategories([FromBody] CategoryFilterDto filterDto)
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
        // POST: /api/admin/categories
        [HttpPost("categories")]
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
        /// Lấy sản phẩm phân trang, lọc, sort
        /// </summary>
        // POST: /api/admin/products/paging
        [HttpPost("products/paging")]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetPagedProducts([FromBody] ProductPagingRequestDto request)
        {
            var result = await _productService.GetPagedProductsAsync(request);
            return Ok(result);
        }

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
        // POST: /api/admin/products
        [HttpPost("products")]
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

        // ================== BRAND ===================
        /// <summary>
        /// Lấy tất cả thương hiệu
        /// </summary>
        // GET: /api/admin/brands
        [HttpGet("brands")]
        public async Task<ActionResult<List<BrandDto>>> GetBrands()
        {
            var result = await _brandService.GetAllBrandsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lọc thương hiệu theo điều kiện
        /// </summary>
        // POST: /api/admin/brands/filter
        [HttpPost("brands/filter")]
        public async Task<ActionResult<List<BrandDto>>> FilterBrands([FromBody] BrandFilterDto filter)
        {
            var result = await _brandService.GetAllBrandsAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Lấy thương hiệu theo ID
        /// </summary>
        // GET: /api/admin/brands/{id}
        [HttpGet("brands/{id}")]
        public async Task<ActionResult<BrandDto>> GetBrandById(Guid id)
        {
            var brand = await _brandService.GetBrandByIdAsync(id);
            if (brand == null) return NotFound();
            return Ok(brand);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật thương hiệu
        /// </summary>
        // POST: /api/admin/brands
        [HttpPost("brands")]
        public async Task<ActionResult<bool>> CreateOrUpdateBrand([FromBody] CreateOrUpdateBrandDto dto)
        {
            if (dto == null) return BadRequest("Brand data is required.");

            var result = await _brandService.CreateOrUpdateBrandAsync(dto);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating or updating brand.");

            return Ok(result);
        }

        /// <summary>
        /// Xóa thương hiệu
        /// </summary>
        // DELETE: /api/admin/brands/{id}
        [HttpDelete("brands/{id}")]
        public async Task<ActionResult<bool>> DeleteBrand(Guid id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }

        // ================== ORDER ===================
        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        // PUT: /api/admin/orders/{id}/status
        [HttpPut("orders/{id}/status")]
      //  [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
        { 
            if (dto == null)
            {
                return BadRequest("Status data is required.");
            }

            var result = await _orderService.UpdateOrderStatusAsync(id, dto.Status);

            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // ================== USER ===================
        /// <summary>
        /// Lấy tất cả người dùng
        /// </summary>
        // GET: /api/admin/users
        [HttpGet("users")]
      //  [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Lấy người dùng theo ID
        /// </summary>
        // GET: /api/admin/users/{id}
        [HttpGet("users/{id}")]
      //  [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật người dùng
        /// </summary>
        // POST: /api/admin/users
        [HttpPost("users")]
       // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateOrUpdateUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is required.");
            }

            var result = await _userService.CreateOrUpdateUserAsync(userDto);
            return Ok(result);
        }

        /// <summary>
        /// Xóa người dùng
        /// </summary>
        // DELETE: /api/admin/users/{id}
        [HttpDelete("users/{id}")]
    //    [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
