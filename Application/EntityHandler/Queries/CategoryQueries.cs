using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.DTOs;
using Application.Interfaces.Queries;

namespace Infrastructure.Queries
{
    public class CategoryQueries : ICategoryQueries
    {
        private readonly EcommerceDbContext _context;

        public CategoryQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .FirstOrDefaultAsync();
        }
    }
}
