using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IBrandService
    {
        // Admin: có filter
        Task<List<BrandInfoDto>> GetAllBrandsAsync(BrandFilterDto filterDto);

        // User: lấy tất cả
        Task<List<BrandInfoDto>> GetAllBrandsAsync();

        // Lấy theo ID
        Task<BrandInfoDto?> GetBrandByIdAsync(Guid id);

        // Tạo mới hoặc cập nhật
        Task<bool> CreateOrUpdateBrandAsync(CreateOrUpdateBrandDto dto);

        // Xoá thương hiệu
        Task<bool> DeleteBrandAsync(Guid id);
    }
}
