using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.EntityHandler.Services
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(Guid userId);

        /// <summary>
        /// Thêm hoặc xóa một sản phẩm khỏi wishlist.
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <param name="productId">ID sản phẩm</param>
        /// <returns>Trả về true nếu sản phẩm được THÊM vào, và false nếu sản phẩm được XÓA.</returns>
        Task<bool> ToggleWishlistAsync(Guid userId, Guid productId);
    }
}
