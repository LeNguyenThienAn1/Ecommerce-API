using Application.EntityHandler.Services;
﻿using Microsoft.AspNetCore.Authorization;
﻿using Microsoft.AspNetCore.Mvc;
﻿using System;
﻿using System.Linq;
﻿using System.Security.Claims;
﻿using System.Threading.Tasks;
﻿
﻿namespace Api.Controllers
﻿{
﻿    [ApiController]
﻿    [Route("api/[controller]")]
﻿    //[Authorize] // Yêu cầu người dùng phải đăng nhập
﻿    public class WishlistController : ControllerBase
﻿    {
﻿        private readonly IWishlistService _wishlistService;
﻿
﻿        public WishlistController(IWishlistService wishlistService)
﻿        {
﻿            _wishlistService = wishlistService;
﻿        }
﻿
﻿        // Lấy Guid của người dùng từ token
﻿        private Guid GetUserIdFromToken()
﻿        {
﻿            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
﻿            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out Guid userId))
﻿            {
﻿                return userId;
﻿            }
﻿            // Ném exception hoặc xử lý lỗi nếu không tìm thấy userId
﻿            throw new InvalidOperationException("User ID not found in token.");
﻿        }
﻿
﻿        /// <summary>
﻿        /// Lấy danh sách sản phẩm yêu thích của người dùng hiện tại.
﻿        /// </summary>
﻿        [HttpGet]
﻿        public async Task<IActionResult> GetWishlist()
﻿        {
﻿            try
﻿            {
﻿                var userId = GetUserIdFromToken();
﻿                var wishlist = await _wishlistService.GetWishlistByUserAsync(userId);
﻿                return Ok(wishlist);
﻿            }
﻿            catch (InvalidOperationException ex)
﻿            {
﻿                return Unauthorized(ex.Message);
﻿            }
﻿        }
﻿
﻿        /// <summary>
﻿        /// Thêm hoặc xóa một sản phẩm khỏi danh sách yêu thích.
﻿        /// </summary>
﻿        /// <param name="productId">ID của sản phẩm cần thêm/xóa.</param>
﻿        [HttpPost("toggle/{productId:guid}")]
﻿        public async Task<IActionResult> ToggleWishlist(Guid productId)
﻿        {
﻿            try
﻿            {
﻿                var userId = GetUserIdFromToken();
﻿                var isAdded = await _wishlistService.ToggleWishlistAsync(userId, productId);
﻿
﻿                if (isAdded)
﻿                {
﻿                    return Ok(new { message = "Product added to wishlist." });
﻿                }
﻿                else
﻿                {
﻿                    return Ok(new { message = "Product removed from wishlist." });
﻿                }
﻿            }
﻿            catch (InvalidOperationException ex)
﻿            {
﻿                return Unauthorized(ex.Message);
﻿            }
﻿            catch (Exception ex)
﻿            {
﻿                // Log the exception details here
﻿                return StatusCode(500, "An unexpected error occurred.");
﻿            }
﻿        }
﻿    }
﻿}
﻿
