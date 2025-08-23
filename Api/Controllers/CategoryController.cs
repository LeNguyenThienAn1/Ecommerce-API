using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces.Services;
using Application.Services;
using Application.DTOs;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => route sẽ là: /api/categories
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Lấy tất cả danh mục (cho User)
        /// </summary>
        // GET: /api/categories
        [HttpGet]
        public async Task<ActionResult<List<CategoryInfoDto>>> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }
    }
}
