using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Application.EntityHandler.Services;
using Infrastructure.Entity;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly EcommerceDbContext _context;
        private readonly IWishlistQueries _wishlistQueries;

        public WishlistService(EcommerceDbContext context, IWishlistQueries wishlistQueries)
        {
            _context = context;
            _wishlistQueries = wishlistQueries;
        }

        public async Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(int userId)
        {
            return await _wishlistQueries.GetWishlistByUserAsync(userId);
        }

        public async Task AddToWishlistAsync(int userId, int productId)
        {
            var exists = await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

            if (!exists)
            {
                var wishlist = new WishlistEntity
                {
                    UserId = userId,
                    ProductId = productId
                };

                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromWishlistAsync(int userId, int productId)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();
            }
        }
    }
}
