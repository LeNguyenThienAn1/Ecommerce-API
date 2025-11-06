//using Application.DTOs;
//using Application.EntityHandler.Queries.Interface;
//using Application.EntityHandler.Services;
//using Infrastructure.Entity;
//using Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace Application.Services
//{
//    public class WishlistService : IWishlistService
//    {
//        private readonly EcommerceDbContext _context;
//        private readonly IWishlistQueries _wishlistQueries;

//        public WishlistService(EcommerceDbContext context, IWishlistQueries wishlistQueries)
//        {
//            _context = context;
//            _wishlistQueries = wishlistQueries;
//        }

//        // Lấy danh sách wishlist theo user
//        public async Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(Guid userId)
//        {
//            return await _wishlistQueries.GetWishlistByUserAsync(userId);
//        }

//        // Thêm sản phẩm vào wishlist
//        public async Task<bool> AddToWishlistAsync(Guid userId, Guid productId)
//        {
//            var exists = await _context.Wishlists
//                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

//            if (exists)
//                return false; // Đã tồn tại, không thêm lại

//            var wishlist = new WishlistEntity
//            {
//                Id = Guid.NewGuid(),
//                UserId = userId,
//                ProductId = productId,
//                CreateAt = DateTime.UtcNow,
//                UpdateAt = DateTime.UtcNow,
//                CreateBy = userId,
//                UpdateBy = userId
//            };

//            _context.Wishlists.Add(wishlist);
//            await _context.SaveChangesAsync();
//            return true;
//        }

//        // Xóa sản phẩm khỏi wishlist
//        public async Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId)
//        {
//            var wishlist = await _context.Wishlists
//                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

//            if (wishlist == null)
//                return false;

//            _context.Wishlists.Remove(wishlist);
//            await _context.SaveChangesAsync();
//            return true;
//        }
//    }
//}
