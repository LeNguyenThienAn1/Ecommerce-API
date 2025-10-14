using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.EntityHandler.Queries
{
    public class WishlistQueries : IWishlistQueries
    {
        private readonly EcommerceDbContext _context;

        public WishlistQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(int userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Product)
                .Select(w => new WishlistDto
                {
                    Id = w.Id,
                    UserId = w.UserId,
                    ProductId = w.ProductId,
                    ProductName = w.Product.Name
                })
                .ToListAsync();
        }
    }
}
