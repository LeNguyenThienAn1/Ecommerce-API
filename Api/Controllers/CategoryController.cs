using Application.DTOs;
using Application.Interfaces.Services;
using Infrastructure; // nếu cần CategoryEntity
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
        /// Lấy tất cả danh mục
        /// </summary>
        // GET: /api/categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryInfoDto>>> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync(null);
            return Ok(result);
        }

        /// <summary>
        /// Tìm kiếm danh mục theo điều kiện filter
        /// </summary>
        // POST: /api/categories/search
        [HttpPost("search")]
        public async Task<ActionResult<List<CategoryInfoDto>>> SearchCategories([FromBody] CategoryFilterDto filterDto)
        {
            var result = await _categoryService.GetAllCategoriesAsync(filterDto);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        // GET: /api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryEntity>> GetCategoryById(Guid id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật danh mục
        /// </summary>
        // POST: /api/categories/createupdate
        [HttpPost("createupdate")]
        public async Task<ActionResult<bool>> CreateOrUpdateCategory([FromBody] CreateOrUpdateCategoryDto dto)
        {
            if (dto == null)
                return BadRequest("Category data is required.");

            var result = await _categoryService.CreateOrUpdateCategoryAsync(dto);
            if (!result)
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating or updating category.");

            return Ok(result);
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        // DELETE: /api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteCategory(Guid id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result) return NotFound();
            return Ok(result);
        }
    }
}
