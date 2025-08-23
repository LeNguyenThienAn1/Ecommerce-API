using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Queries;
using Application;

namespace Infrastructure
{
    public class CategoryQueries : ICategoryQueries
    {
        private readonly EcommerceDbContext _context;

        public CategoryQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryEntity>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<CategoryEntity> GetCategoryByIdAsync(Guid id)
        {
            var result = await _context.Categories.FindAsync(id);
            return result;
        }

        public async Task<bool> CreateCategoryAsync(CategoryEntity entity)
        {
            entity.Id = Guid.NewGuid();
            _context.Categories.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryEntity entity)
        {
            _context.Categories.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(Guid id)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return false;

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

