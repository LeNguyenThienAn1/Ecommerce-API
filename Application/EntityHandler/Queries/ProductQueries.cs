using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Queries;


namespace Infrastructure.Queries
{
    public class ProductQueries : IProductQueries
    {
        private readonly EcommerceDbContext _context;

        public ProductQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync()
        {
            try
            {
                var productEntities = await _context.Products.DistinctBy(x => x.CategoryId).ToListAsync();
                return productEntities;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw; // Re-throw the exception after logging it
            }
        }
        public async Task<ProductEntity> GetProductByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<bool> CreateProductAsync(List<ProductEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.Id = Guid.NewGuid();
                entity.CreateAt = DateTime.UtcNow;
                entity.Status = ProductStatus.Available;
            }
            _context.Products.AddRange(entities);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateProductAsync(ProductEntity entity)
            {
            _context.Products.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return false;

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

