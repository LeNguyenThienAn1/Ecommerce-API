using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.EntityHandler.Services
{
    public interface IProductCommentService
    {
        Task<ProductCommentDto> CreateComment(CreateProductCommentDto commentDto, Guid userId);
        Task<ProductCommentDto> UpdateComment(Guid commentId, UpdateProductCommentDto commentDto, Guid userId);
        Task<bool> DeleteComment(Guid commentId, Guid userId);
    }
}
