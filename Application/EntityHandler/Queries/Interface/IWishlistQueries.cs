using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.EntityHandler.Queries.Interface
{
    public interface IWishlistQueries
    {
        Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(Guid userId);
    }
}
