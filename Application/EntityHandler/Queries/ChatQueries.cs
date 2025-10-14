using Application.DTOs;
using EntityHandler.Queries.Interface;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EntityHandler.Queries
{
    public class ChatQueries : IChatQueries
    {
        private readonly EcommerceDbContext _context;

        public ChatQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> SearchProductsAsync(string keyword)
        {
            return await _context.Products
                .Where(p => p.Name.ToLower().Contains(keyword.ToLower())
                         || p.Description.ToLower().Contains(keyword.ToLower()))
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    SalePercent = p.SalePercent,
                    Category = p.Category,
                    Brand = p.Brand,
                })
                .ToListAsync();
        }
    }
}
