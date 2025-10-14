using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Application.Interfaces.Queries;

namespace Infrastructure.Queries
{
    public class BrandQueries : IBrandQueries
    {
        private readonly EcommerceDbContext _context;

        public BrandQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BrandDto>> GetAllBrandsAsync()
        {
            return await _context.Brands
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    LogoUrl = b.LogoUrl
                })
                .ToListAsync();
        }

        public async Task<BrandDto?> GetBrandByIdAsync(Guid id)
        {
            return await _context.Brands
                .Where(b => b.Id == id)
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    LogoUrl = b.LogoUrl
                })
                .FirstOrDefaultAsync();
        }
    }
}
