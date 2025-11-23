using Application.DTOs;
using Application.EntityHandler.Services;
using Infrastructure;
using Infrastructure.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ProductCommentService : IProductCommentService
    {
        private readonly EcommerceDbContext _context;

        public ProductCommentService(EcommerceDbContext context)
        {
            _context = context;
        }

        public async Task<ProductCommentDto> CreateComment(CreateProductCommentDto commentDto, Guid userId)
        {
            var product = await _context.Products.FindAsync(commentDto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var comment = new ProductCommentEntity
            {
                ProductId = commentDto.ProductId,
                UserId = userId,
                Content = commentDto.Content,
                CreateAt = DateTime.UtcNow
            };

            _context.ProductComments.Add(comment);
            await _context.SaveChangesAsync();

            return new ProductCommentDto
            {
                Id = comment.Id,
                ProductId = comment.ProductId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedDate = comment.CreateAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    AvatarUrl = user.AvatarUrl
                }
            };
        }

        public async Task<ProductCommentDto> UpdateComment(Guid commentId, UpdateProductCommentDto commentDto, Guid userId)
        {
            var comment = await _context.ProductComments.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            if (comment.UserId != userId)
            {
                throw new Exception("You are not authorized to edit this comment");
            }

            comment.Content = commentDto.Content;
            comment.UpdateAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ProductCommentDto
            {
                Id = comment.Id,
                ProductId = comment.ProductId,
                UserId = comment.UserId,
                Content = comment.Content,
                CreatedDate = comment.CreateAt,
                User = new UserDto
                {
                    Id = comment.User.Id,
                    Name = comment.User.Name,
                    AvatarUrl = comment.User.AvatarUrl
                }
            };
        }

        public async Task<bool> DeleteComment(Guid commentId, Guid userId)
        {
            var comment = await _context.ProductComments.FindAsync(commentId);
            if (comment == null)
            {
                throw new Exception("Comment not found");
            }

            if (comment.UserId != userId)
            {
                throw new Exception("You are not authorized to delete this comment");
            }

            _context.ProductComments.Remove(comment);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
