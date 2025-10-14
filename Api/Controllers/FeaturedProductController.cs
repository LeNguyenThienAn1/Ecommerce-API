using EntityHandler.Services.Interface; // 👈 chú ý namespace
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturedProductController : ControllerBase
    {
        private readonly IFeaturedProductService _featuredProductService;

        public FeaturedProductController(IFeaturedProductService featuredProductService)
        {
            _featuredProductService = featuredProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            var products = await _featuredProductService.GetFeaturedProductsAsync();
            return Ok(products);
        }
    }
}
