using Application.DTOs;
﻿using Application.EntityHandler.Queries.Interface;
using Infrastructure;
using Infrastructure.Data;
﻿using Microsoft.EntityFrameworkCore;
﻿using System;
﻿using System.Collections.Generic;
﻿using System.Linq;
﻿using System.Threading.Tasks;
﻿
﻿namespace Application.EntityHandler.Queries
﻿{
﻿    public class WishlistQueries : IWishlistQueries
﻿    {
﻿        private readonly EcommerceDbContext _context;
﻿
﻿        public WishlistQueries(EcommerceDbContext context)
﻿        {
﻿            _context = context;
﻿        }
﻿
﻿        public async Task<IEnumerable<WishlistDto>> GetWishlistByUserAsync(Guid userId)
﻿        {
﻿            return await _context.Wishlists
﻿                .AsNoTracking()
﻿                .Where(w => w.UserId == userId)
﻿                .Select(w => new WishlistDto
﻿                {
﻿                    UserId = w.UserId,
﻿                    ProductId = w.ProductId,
﻿                    // Lấy thông tin từ Product liên quan
﻿                    Id = w.Product.Id, // Lấy Id của sản phẩm
﻿                    ProductName = w.Product.Name,
﻿                    Price = w.Product.Price,
﻿                    ImageUrl = w.Product.ImageUrl,
﻿                    ProductDescription = w.Product.Description,
                    BrandId = w.Product.BrandId,
                    CategoryId = w.Product.CategoryId,
                    BrandName = w.Product.Brand != null ? w.Product.Brand.Name : null,
                    CategoryName = w.Product.Category != null ? w.Product.Category.Name : null
                })
﻿                .ToListAsync();
﻿        }
﻿    }
﻿}
﻿
