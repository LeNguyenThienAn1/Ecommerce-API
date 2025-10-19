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

            // 1. CHIA TỪ KHÓA THÀNH TỪ RIÊNG LẺ
            // Ví dụ: "điện thoại samsung mới nhất" -> ["điện thoại", "samsung", "mới", "nhất"]
            var searchTerms = keyword.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(t => t.Trim().ToLowerInvariant())
                                     .ToList();

            // Nếu không có từ khóa nào, trả về rỗng
            if (!searchTerms.Any())
            {
                return new List<ProductDto>();
            }

            // Bắt đầu truy vấn
            var query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsNoTracking()
                .AsQueryable(); // Khởi tạo IQueryable

            // 2. TẠO ĐIỀU KIỆN WHERE ĐỘNG (OR logic)
            // Tìm kiếm sản phẩm mà tên, mô tả, thương hiệu, HOẶC danh mục khớp với BẤT KỲ từ khóa nào
            foreach (var term in searchTerms)
            {
                // Thêm điều kiện OR: p.Name LIKE %term% HOẶC p.Description LIKE %term%
                query = query.Where(p =>
                    EF.Functions.Like(p.Name.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(p.Description.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(p.Brand.Name.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(p.Category.Name.ToLower(), $"%{term}%")
                );
            }
            // 🛑 LƯU Ý: Logic trên sử dụng toán tử AND giữa các từ (ví dụ: tìm cả "điện thoại" AND "samsung").
            // Để tìm sản phẩm khớp với BẤT KỲ từ nào (OR logic), ta phải dùng PredicateBuilder hoặc Where (p => true)
            // hoặc dùng cách đơn giản hóa sau đây:

            // Cách đơn giản hơn: Kiểm tra xem TÊN có chứa BẤT KỲ từ khóa nào KHÔNG
            query = _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .AsNoTracking()
                .Where(p => searchTerms.Any(term =>
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    p.Brand.Name.ToLower().Contains(term) ||
                    p.Category.Name.ToLower().Contains(term)
                ));


            // 3. Thực hiện truy vấn và mapping sang DTO
            return await query
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