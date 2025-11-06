//using Application.EntityHandler.Services;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Threading.Tasks;

//namespace Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class WishlistController : ControllerBase
//    {
//        private readonly IWishlistService _wishlistService;

//        public WishlistController(IWishlistService wishlistService)
//        {
//            _wishlistService = wishlistService;
//        }

//        // Lấy danh sách wishlist theo userId
//        [HttpGet("{userId:guid}")]
//        public async Task<IActionResult> GetWishlist(Guid userId)
//        {
//            var wishlist = await _wishlistService.GetWishlistByUserAsync(userId);
//            if (wishlist == null)
//                return NotFound("Wishlist not found.");

//            return Ok(wishlist);
//        }

//        // Thêm sản phẩm vào wishlist
//        [HttpPost("{userId:guid}/{productId:guid}")]
//        public async Task<IActionResult> AddToWishlist(Guid userId, Guid productId)
//        {
//            var result = await _wishlistService.AddToWishlistAsync(userId, productId);
//            if (!result)
//                return BadRequest("Failed to add to wishlist (maybe already exists).");

//            return Ok("Added to wishlist");
//        }

//        // Xóa sản phẩm khỏi wishlist
//        [HttpDelete("{userId:guid}/{productId:guid}")]
//        public async Task<IActionResult> RemoveFromWishlist(Guid userId, Guid productId)
//        {
//            var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
//            if (!result)
//                return NotFound("Wishlist item not found.");

//            return Ok("Removed from wishlist");
//        }
//    }
//}
