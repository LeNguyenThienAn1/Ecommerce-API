using Application.DTOs;

namespace EntityHandler.Services.Interface
{
    public interface IFeaturedProductService
    {
        Task<IEnumerable<FeaturedProductDto>> GetFeaturedProductsAsync();
    }
}
