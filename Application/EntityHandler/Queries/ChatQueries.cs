using Application.DTOs;
using EntityHandler.Queries.Interface;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            if (string.IsNullOrWhiteSpace(keyword))
                return new List<ProductDto>();

            // 1. Chuẩn hóa và Tách từ khóa (Thực thi trên RAM, dùng ToLowerInvariant an toàn)
            var searchTerms = keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(t => t.Trim().ToLowerInvariant())
                                     .Where(t => t.Length >= 2) // Lọc từ quá ngắn
                                     .ToList();

            if (!searchTerms.Any())
            {
                return new List<ProductDto>();
            }

            // 2. Xây dựng Truy vấn TỐI ƯU HÓA (OR Logic)
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => searchTerms.Any(term =>
                    // SỬA LỖI: Sử dụng .ToLower() thay cho .ToLowerInvariant() để EF Core dịch sang SQL
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    p.Brand.Name.ToLower().Contains(term) ||
                    p.Category.Name.ToLower().Contains(term)
                ));

            // 3. Thực hiện truy vấn, ưu tiên sản phẩm có tên khớp với từ khóa gốc
            var normalizedKeyword = keyword.ToLower();

            return await query
                .OrderByDescending(p => p.Name.ToLower().Contains(normalizedKeyword)) // Ưu tiên tên khớp
                .ThenByDescending(p => p.Price) // Sắp xếp phụ (giá cao trước)
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
                .Take(10)
                .ToListAsync();
        }

        /// <summary>
        /// Đếm tổng số lượng sản phẩm khớp với từ khóa tìm kiếm.
        /// </summary>
        public async Task<int> GetProductCountAsync(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return 0;

            // Chuẩn hóa trên RAM, dùng ToLowerInvariant an toàn
            var searchTerms = keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(t => t.Trim().ToLowerInvariant())
                                     .Where(t => t.Length >= 2)
                                     .ToList();

            if (!searchTerms.Any()) return 0;

            return await _context.Products
                .AsNoTracking()
                .CountAsync(p => searchTerms.Any(term =>
                    // SỬA LỖI: Sử dụng .ToLower() thay cho .ToLowerInvariant() để EF Core dịch sang SQL
                    p.Name.ToLower().Contains(term) ||
                    p.Brand.Name.ToLower().Contains(term) ||
                    p.Category.Name.ToLower().Contains(term)
                ));
        }

        public async Task<List<ProductDto>> FindProductsByCriteriaAsync(string? keywords, string? category, string? brand, decimal? minPrice,
            decimal? maxPrice, Infrastructure.ProductSortBy sortBy)
        {
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsNoTracking();

            // Lọc theo từ khóa
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                var searchTerms = keywords.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(t => t.Trim().ToLower())
                                           .Where(t => t.Length >= 2)
                                           .ToList();
                if (searchTerms.Any())
                {
                    query = query.Where(p => searchTerms.Any(term =>
                        p.Name.ToLower().Contains(term) ||
                        p.Description.ToLower().Contains(term)
                    ));
                }
            }

            // Lọc theo danh mục
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category.Name.ToLower().Contains(category.ToLower()));
            }

            // Lọc theo thương hiệu
            if (!string.IsNullOrWhiteSpace(brand))
            {
                query = query.Where(p => p.Brand.Name.ToLower().Contains(brand.ToLower()));
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Sắp xếp
            IOrderedQueryable<Infrastructure.ProductEntity> orderedQuery;
            switch (sortBy)
            {
                case Infrastructure.ProductSortBy.PriceAsc:
                    orderedQuery = query.OrderBy(p => p.Price).ThenByDescending(p => p.CreateAt);
                    break;
                case Infrastructure.ProductSortBy.PriceDesc:
                    orderedQuery = query.OrderByDescending(p => p.Price).ThenByDescending(p => p.CreateAt);
                    break;
                case Infrastructure.ProductSortBy.Newest:
                    orderedQuery = query.OrderByDescending(p => p.CreateAt);
                    break;
                case Infrastructure.ProductSortBy.Relevance:
                default:
                    if (!string.IsNullOrWhiteSpace(keywords))
                    {
                        var normalizedKeyword = keywords.ToLower();
                        orderedQuery = query.OrderByDescending(p => p.Name.ToLower().Contains(normalizedKeyword))
                                            .ThenByDescending(p => p.Price);
                    }
                    else
                    {
                        // Mặc định là mới nhất nếu không có keyword cho độ liên quan
                        orderedQuery = query.OrderByDescending(p => p.CreateAt);
                    }
                    break;
            }

            return await orderedQuery
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
                .Take(10)
                .ToListAsync();
        }
    }
}   