using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using Application.EntityHandler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductCommentController : ControllerBase
    {
        private readonly IProductCommentService _commentService;
        private readonly IProductCommentQueries _commentQueries;

        public ProductCommentController(IProductCommentService commentService, IProductCommentQueries commentQueries)
        {
            _commentService = commentService;
            _commentQueries = commentQueries;
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<IEnumerable<ProductCommentDto>>> GetComments(Guid productId)
        {
            var comments = await _commentQueries.GetCommentsByProductId(productId);
            return Ok(comments);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ProductCommentDto>> CreateComment(CreateProductCommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var createdComment = await _commentService.CreateComment(commentDto, Guid.Parse(userId));
            return Ok(createdComment);
        }

        [HttpPut("{commentId}")]
        [Authorize]
        public async Task<ActionResult<ProductCommentDto>> UpdateComment(Guid commentId, UpdateProductCommentDto commentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var updatedComment = await _commentService.UpdateComment(commentId, commentDto, Guid.Parse(userId));
                return Ok(updatedComment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<ActionResult<bool>> DeleteComment(Guid commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _commentService.DeleteComment(commentId, Guid.Parse(userId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
