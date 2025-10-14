using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Queries
{
    public interface IBrandQueries
    {
        // Lấy tất cả thương hiệu
        Task<IEnumerable<BrandDto>> GetAllBrandsAsync();

        // Lấy thương hiệu theo ID
        Task<BrandDto?> GetBrandByIdAsync(Guid id);
    }
}
