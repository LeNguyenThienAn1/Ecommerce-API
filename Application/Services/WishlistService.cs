using Application.DTOs;
﻿using Application.EntityHandler.Queries.Interface;
﻿using Application.EntityHandler.Services;
using Infrastructure;
using Infrastructure.Data;
﻿using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System;
﻿using System.Collections.Generic;
﻿using System.Threading.Tasks;
﻿
﻿namespace Application.Services
﻿{
﻿    public class WishlistService : IWishlistService
﻿    {
﻿        private readonly EcommerceDbContext _context;
﻿        private readonly IWishlistQueries _wishlistQueries;
﻿
﻿        public WishlistService(EcommerceDbContext context, IWishlistQueries wishlistQueries)
﻿        {
﻿            _context = context;
﻿            _wishlistQueries = wishlistQueries;
﻿        }
﻿
﻿        public async Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(Guid userId)
﻿        {
﻿            return await _wishlistQueries.GetWishlistByUserAsync(userId);
﻿        }
﻿
﻿        public async Task<bool> ToggleWishlistAsync(Guid userId, Guid productId)
﻿        {
﻿            var wishlistItem = await _context.Wishlists
﻿                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
﻿
﻿            if (wishlistItem != null)
﻿            {
﻿                // Nếu đã tồn tại -> Xóa
﻿                _context.Wishlists.Remove(wishlistItem);
﻿                await _context.SaveChangesAsync();
﻿                return false; // Trả về false vì đã xóa
﻿            }
﻿            else
﻿            {
﻿                // Nếu chưa tồn tại -> Thêm mới
﻿                var newWishlistItem = new WishlistEntity
﻿                {
﻿                    UserId = userId,
﻿                    ProductId = productId
﻿                };
﻿                _context.Wishlists.Add(newWishlistItem);
﻿                await _context.SaveChangesAsync();
﻿                return true; // Trả về true vì đã thêm
﻿            }
﻿        }
﻿    }
﻿}
﻿
