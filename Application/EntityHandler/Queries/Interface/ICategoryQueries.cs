using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces.Queries
{
    public interface ICategoryQueries
    {
        // Lấy tất cả danh mục
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();

        // Lấy danh mục theo ID
        Task<CategoryDto?> GetCategoryByIdAsync(Guid id);
    }
}
