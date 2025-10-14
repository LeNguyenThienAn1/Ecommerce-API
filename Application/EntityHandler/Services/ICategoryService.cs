using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Dành cho admin - Lấy danh sách danh mục có filter
        /// </summary>
        Task<List<CategoryInfoDto>> GetAllCategoriesAsync(CategoryFilterDto? filterDto);

        /// <summary>
        /// Dành cho user - Lấy tất cả danh mục (không filter)
        /// </summary>
        Task<List<CategoryInfoDto>> GetAllCategoriesAsync();

        /// <summary>
        /// Lấy thông tin danh mục theo ID
        /// </summary>
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);

        /// <summary>
        /// Tạo mới hoặc cập nhật danh mục
        /// </summary>
        Task<bool> CreateOrUpdateCategoryAsync(CreateOrUpdateCategoryDto dto);

        /// <summary>
        /// Xóa danh mục theo ID
        /// </summary>
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
