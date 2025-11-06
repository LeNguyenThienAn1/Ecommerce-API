using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Queries;
using Application.DTOs;
using Infrastructure.Entity;

namespace Infrastructure.Queries
{
    public class ProductQueries : IProductQueries
    {
        private readonly EcommerceDbContext _context;

        public ProductQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả sản phẩm (có phân trang + lọc + sắp xếp)
        /// </summary>
        public async Task<PagedResult<ProductDto>> GetAllProductsAsync(ProductPagingRequestDto request)
        {
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Status == ProductStatus.Available)
                .AsQueryable();

            // 🔍 --- Search ---
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                string search = request.SearchTerm.Trim().ToLower();
                query = query.Where(p => p.Name.ToLower().Contains(search));
            }

            // 🏭 --- Filter by Brand ---
            if (request.BrandIds != null && request.BrandIds.Any(id => id != Guid.Empty))
            {
                query = query.Where(p => request.BrandIds.Contains(p.BrandId));
            }

            // 🏷️ --- Filter by Category ---
            if (request.CategoryIds != null && request.CategoryIds.Any(id => id != Guid.Empty))
            {
                query = query.Where(p => request.CategoryIds.Contains(p.CategoryId));
            }

            // 🔢 --- Tổng số bản ghi ---
            int totalCount = await query.CountAsync();

            // ⏱️ --- Sort ---
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                switch (request.SortBy.ToLower())
                {
                    case "name":
                        query = query.OrderBy(p => p.Name);
                        break;
                    case "name_desc":
                        query = query.OrderByDescending(p => p.Name);
                        break;
                    case "price":
                        query = query.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        query = query.OrderByDescending(p => p.Price);
                        break;
                    case "createdat":
                        query = query.OrderBy(p => p.CreateAt);
                        break;
                    case "createdat_desc":
                        query = query.OrderByDescending(p => p.CreateAt);
                        break;
                    default:
                        query = query.OrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                query = query.OrderBy(p => p.Name);
            }

            // 📄 --- Paging ---
            int pageNumber = request.PageNumber <= 0 ? 1 : request.PageNumber;
            int pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
            int skip = (pageNumber - 1) * pageSize;

            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsFeatured = p.IsFeatured,
                    FeaturedType = p.FeaturedType,
                    SalePercent = p.SalePercent,
                    CategoryId = p.CategoryId,
                    BrandId = p.BrandId,
                    CreateAt = p.CreateAt,
                    Detail = p.Detail
                })
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// Lấy tất cả Brand
        /// </summary>
        public async Task<List<BrandDto>> GetAllBrandAsync()
        {
            return await _context.Brands
                .AsNoTracking()
                .Select(b => new BrandDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync();
        }

        /// <summary>
        /// Lấy sản phẩm theo Id (bao gồm Category & Brand)
        /// </summary>
        public async Task<ProductEntity> GetProductByIdAsync(Guid id)
        {
            return await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Tạo mới nhiều sản phẩm
        /// </summary>
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

        /// <summary>
        /// Cập nhật sản phẩm
        /// </summary>
        public async Task<bool> UpdateProductAsync(ProductEntity entity)
        {
            _context.Products.Update(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Xóa sản phẩm theo Id
        /// </summary>
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
