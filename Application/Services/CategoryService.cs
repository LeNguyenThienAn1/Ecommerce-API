using Application.DTOs;
using Application.Interfaces.Queries;
using Application.Interfaces.Services;
using Infrastructure;
using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryQueries _categoryQueries;
        private readonly EcommerceDbContext _context;

        public CategoryService(ICategoryQueries categoryQueries, EcommerceDbContext context)
        {
            _categoryQueries = categoryQueries;
            _context = context;
        }

        // ====================== ADMIN - Lấy tất cả danh mục có filter ======================
        public async Task<List<CategoryInfoDto>> GetAllCategoriesAsync(CategoryFilterDto? filter)
        {
            var query = _context.Categories.AsQueryable();

            // 🔍 Filter theo tên
            if (!string.IsNullOrEmpty(filter?.CategoryName))
            {
                query = query.Where(c => c.Name.Contains(filter.CategoryName));
            }

            // 🔍 Có thể mở rộng thêm filter theo ngày tạo, mô tả...
            // if (!string.IsNullOrEmpty(filter?.Description))
            //     query = query.Where(c => c.Description.Contains(filter.Description));

            var result = await query
                .OrderBy(c => c.Name)
                .Select(c => new CategoryInfoDto
                {
                    CategoryId = c.Id,
                    CategoryName = c.Name,
                    Description = c.Description
                })
                .ToListAsync();

            return result;
        }

        // ====================== USER - Lấy tất cả danh mục (không filter) ======================
        public async Task<List<CategoryInfoDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryQueries.GetAllCategoriesAsync();
            return categories.Select(c => new CategoryInfoDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
            }).ToList();
        }

        // ====================== Lấy danh mục theo ID ======================
        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            var cat = await _categoryQueries.GetCategoryByIdAsync(id);
            if (cat == null) return null;

            return new CategoryDto
            {
                Name = cat.Name,
                Description = cat.Description
            };
        }

        // ====================== Tạo hoặc cập nhật danh mục ======================
        public async Task<bool> CreateOrUpdateCategoryAsync(CreateOrUpdateCategoryDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên danh mục không được để trống.");

            // 🧩 Kiểm tra userId
            if (dto.UpdateBy == Guid.Empty && dto.CreateBy == Guid.Empty)
                throw new ArgumentException("Thiếu thông tin UserId người thực hiện.");

            if (dto.Id == Guid.Empty)
            {
                // 🆕 CREATE
                var entity = new CategoryEntity
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name.Trim(),
                    Description = dto.Description?.Trim(),
                    CreateBy = dto.UserId, // ✅ Gán người tạo
                    UpdateBy = dto.UserId,
                };

                await _context.Categories.AddAsync(entity);
            }
            else
            {
                // ✏️ UPDATE
                var entity = await _context.Categories.FindAsync(dto.Id);
                if (entity == null) return false;

                entity.Name = dto.Name.Trim();
                entity.Description = dto.Description?.Trim();
                entity.UpdateAt = DateTime.UtcNow;
                entity.UpdateBy = dto.UserId;// ✅ Gán người cập nhật

                _context.Categories.Update(entity);
            }

            return await _context.SaveChangesAsync() > 0;
        }


        // ====================== Xóa danh mục ======================
        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var entity = await _context.Categories.FindAsync(id);
            if (entity == null) return false;

            _context.Categories.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
