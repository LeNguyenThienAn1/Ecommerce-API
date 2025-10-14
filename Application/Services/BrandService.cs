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
    public class BrandService : IBrandService
    {
        private readonly IBrandQueries _brandQueries;
        private readonly EcommerceDbContext _context;

        public BrandService(IBrandQueries brandQueries, EcommerceDbContext context)
        {
            _brandQueries = brandQueries;
            _context = context;
        }

        // Admin: có filter
        public async Task<List<BrandInfoDto>> GetAllBrandsAsync(BrandFilterDto filter)
        {
            var query = _context.Brands.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                query = query.Where(b => b.Name.Contains(filter.Keyword));
            }

            var result = await query
                .Select(b => new BrandInfoDto
                {
                    BrandId = b.Id,
                    BrandName = b.Name,
                    LogoUrl = b.LogoUrl
                })
                .ToListAsync();

            return result;
        }

        // User: lấy tất cả
        public async Task<List<BrandInfoDto>> GetAllBrandsAsync()
        {
            var brands = await _brandQueries.GetAllBrandsAsync();
            return brands
                .Select(b => new BrandInfoDto
                {
                    BrandId = b.Id,
                    BrandName = b.Name,
                    LogoUrl = b.LogoUrl
                })
                .ToList();
        }

        // Get by Id
        public async Task<BrandInfoDto?> GetBrandByIdAsync(Guid id)
        {
            var brand = await _brandQueries.GetBrandByIdAsync(id);
            if (brand == null) return null;

            return new BrandInfoDto
            {
                BrandId = brand.Id,
                BrandName = brand.Name,
                LogoUrl = brand.LogoUrl
            };
        }

        // Create or Update
        public async Task<bool> CreateOrUpdateBrandAsync(CreateOrUpdateBrandDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Tên thương hiệu không được để trống.");

            if (dto.Id == Guid.Empty)
            {
                var entity = new BrandEntity
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    LogoUrl = dto.LogoUrl
                };
                _context.Brands.Add(entity);
            }
            else
            {
                var entity = await _context.Brands.FindAsync(dto.Id);
                if (entity == null) return false;

                entity.Name = dto.Name;
                entity.LogoUrl = dto.LogoUrl;
                _context.Brands.Update(entity);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        // Delete
        public async Task<bool> DeleteBrandAsync(Guid id)
        {
            var entity = await _context.Brands.FindAsync(id);
            if (entity == null) return false;

            _context.Brands.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
