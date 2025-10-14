using Application.EntityHandler.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetWishlist(int userId)
        {
            var wishlist = await _wishlistService.GetWishlistByUserAsync(userId);
            return Ok(wishlist);
        }

        [HttpPost("{userId}/{productId}")]
        public async Task<IActionResult> AddToWishlist(int userId, int productId)
        {
            await _wishlistService.AddToWishlistAsync(userId, productId);
            return Ok("Added to wishlist");
        }

        [HttpDelete("{userId}/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int userId, int productId)
        {
            await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            return Ok("Removed from wishlist");
        }
    }
}
