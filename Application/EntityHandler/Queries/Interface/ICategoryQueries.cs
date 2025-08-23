using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;
using Infrastructure;

namespace Application
{
    public interface ICategoryQueries
    {
        // Lấy tất cả danh mục
        Task<IEnumerable<CategoryEntity>> GetAllCategoriesAsync();

        // Lấy danh mục theo ID
        Task<CategoryEntity> GetCategoryByIdAsync(Guid id);

        // Tạo mới danh mục
        Task<bool> CreateCategoryAsync(CategoryEntity entity);

        // Cập nhật danh mục
        Task<bool> UpdateCategoryAsync(CategoryEntity entity);

        // Xoá danh mục
        Task<bool> DeleteCategoryAsync(Guid id);
    }
}
