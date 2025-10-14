using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EntityHandler.Queries
{
    public class FeaturedProductQueries : IFeaturedProductQueries
    {
        private readonly EcommerceDbContext _context;

        public FeaturedProductQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeaturedProductDto>> GetFeaturedProductsAsync()
        {
            return await _context.Products
                .Where(p => p.IsFeatured && p.Status == ProductStatus.Available)
                .Select(p => new FeaturedProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    SalePercent = p.SalePercent,
                    Brand = p.Brand.ToString(),
                    Category = p.Category.ToString(),
                    IsFeatured = p.IsFeatured,
                    FeaturedType = p.FeaturedType.ToString()
                })
                .ToListAsync();
        }
    }
}
