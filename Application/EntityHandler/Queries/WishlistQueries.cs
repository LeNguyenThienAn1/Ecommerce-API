//using Application.DTOs;
//using Application.EntityHandler.Queries.Interface;
//using Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Application.EntityHandler.Queries
//{
//    public class WishlistQueries : IWishlistQueries
//    {
//        private readonly EcommerceDbContext _context;

//        public WishlistQueries(EcommerceDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(Guid userId)
//        {
//            return await _context.Wishlists
//                .Where(w => w.UserId == userId)
//                .Include(w => w.Product)
//                .Select(w => new WishlistDto
//                {
//                    Id = w.Id,
//                    UserId = w.UserId,
//                    ProductId = w.ProductId,
//                    ProductName = w.Product.Name,
//                    Price = w.Product.Price,
//                    ImageUrl = w.Product.ImageUrl,
//                })
//                .ToListAsync();
//        }
//    }
//}
