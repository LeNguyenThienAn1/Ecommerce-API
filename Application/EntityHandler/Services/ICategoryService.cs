using Application.DTOs;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICategoryService
    {
        // Admin: có filter
        Task<List<CategoryInfoDto>> GetAllCategoriesAsync(CategoryFilterDto filterDto);
        // User: lấy tất cả
        Task<List<CategoryInfoDto>> GetAllCategoriesAsync();
        Task<CategoryEntity> GetCategoryByIdAsync(Guid id);
        Task<bool> CreateOrUpdateCategoryAsync(CreateOrUpdateCategoryDto dto);

        // Xoá danh mục
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
