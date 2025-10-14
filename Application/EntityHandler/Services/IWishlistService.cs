using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.EntityHandler.Services
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(int userId);
        Task AddToWishlistAsync(int userId, int productId);
        Task RemoveFromWishlistAsync(int userId, int productId);
    }
}
