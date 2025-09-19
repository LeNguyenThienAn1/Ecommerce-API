using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Queries;
using Application.DTOs;
using Microsoft.EntityFrameworkCore.Internal;


namespace Infrastructure.Queries
{
    public class ProductQueries : IProductQueries
    {
        private readonly EcommerceDbContext _context;

        public ProductQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<ProductDto>> GetAllProductsAsync(ProductPagingRequestDto request)
        {
            var query = _context.Products.Where(p => p.Status == ProductStatus.Available);

            // --- Search ---
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string search = request.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search));
            }

            // --- Filter by Brand ---
            if (request.Brand != null && request.Brand.Any())
            {
                query = query.Where(p => request.Brand.Contains(p.Brand));
            }

            // --- Filter by Category ---
            if (request.Category != null && request.Category.Any())
            {
                query = query.Where(p => request.Category.Contains(p.Category));
            }

            // --- Tổng số bản ghi ---
            int totalCount = await query.CountAsync();

            // --- Sort ---
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "name": query = query.OrderBy(p => p.Name); break;
                    case "name_desc": query = query.OrderByDescending(p => p.Name); break;
                    case "price": query = query.OrderBy(p => p.Price); break;
                    case "price_desc": query = query.OrderByDescending(p => p.Price); break;
                    case "createdat": query = query.OrderBy(p => p.CreateAt); break;
                    case "createdat_desc": query = query.OrderByDescending(p => p.CreateAt); break;
                    default: query = query.OrderBy(p => p.Name); break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }

            // --- Paging ---
            int skip = (request.PageNumber - 1) * request.PageSize;
            var items = await query
                .Skip(skip)
                .Take(request.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Brand = p.Brand,
                    Category = p.Category,
                    CreateAt = p.CreateAt,
                    ImageUrl = p.ImageUrl,
                    IsFeatured = p.IsFeatured,
                    FeaturedType = p.FeaturedType,
                    SalePercent = p.SalePercent
                })
                .AsNoTracking()
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }




        //public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync(ProductPagingRequestDto request)
        //{
        //    try
        //    {
        //        var productEntities = await _context.Products.ToListAsync();
        //        return productEntities;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred: {ex.Message}");
        //        throw;
        //    }
        //}

        public async Task<List<BrandDto>> GetAllBrandAsync()
        {
            var brands = await _context.Products
                                       .Select(p => p.Brand)
                                       .Distinct()
                                       .ToListAsync();

            var brandDtos = brands.Select(b => new BrandDto { Name = b.ToString() }).ToList();
            return brandDtos;
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
            await _context.Products.AddRangeAsync(entities);
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

