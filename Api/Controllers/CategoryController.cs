using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => /api/categories
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Lấy tất cả danh mục (cho user)
        /// </summary>
        // GET: /api/categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryInfoDto>>> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm hoặc lọc danh mục (cho admin)
        /// </summary>
        // POST: /api/categories/search
        [HttpPost("search")]
        public async Task<ActionResult<List<CategoryInfoDto>>> SearchCategories([FromBody] CategoryFilterDto filterDto)
        {
            if (filterDto == null)
                filterDto = new CategoryFilterDto();

            var result = await _categoryService.GetAllCategoriesAsync(filterDto);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        // GET: /api/categories/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoryInfoDto>> GetCategoryById(Guid id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result == null)
                return NotFound($"Không tìm thấy danh mục với ID: {id}");
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật danh mục
        /// </summary>
        // POST: /api/categories
        [HttpPost]
        public async Task<ActionResult> CreateOrUpdateCategory([FromBody] CreateOrUpdateCategoryDto dto)
        {
            if (dto == null)
                return BadRequest("Category data is required.");

            var success = await _categoryService.CreateOrUpdateCategoryAsync(dto);
            if (!success)
                return StatusCode(StatusCodes.Status500InternalServerError, "Lỗi khi tạo hoặc cập nhật danh mục.");

            return Ok(new { success = true });
        }

        /// <summary>
        /// Xóa danh mục theo ID
        /// </summary>
        // DELETE: /api/categories/{id}
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound($"Không tìm thấy danh mục có ID: {id}");

            return Ok(new { success = true });
        }
    }
}
