using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.EntityHandler.Queries
{
    public class ProductCommentQueries : IProductCommentQueries
    {
        private readonly EcommerceDbContext _context;

        public ProductCommentQueries(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductCommentDto>> GetCommentsByProductId(Guid productId)
        {
            var comments = await _context.ProductComments
                .Include(c => c.User)
                .Where(c => c.ProductId == productId && !c.IsDelete)
                .OrderByDescending(c => c.CreateAt)
                .Select(c => new ProductCommentDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedDate = c.CreateAt,

                    User = new UserDto
                    {
                        Id = c.User.Id,
                        Name = c.User.Name,
                        AvatarUrl = c.User.AvatarUrl
                    }
                })
                .ToListAsync();

            return comments;
        }
    }
}
