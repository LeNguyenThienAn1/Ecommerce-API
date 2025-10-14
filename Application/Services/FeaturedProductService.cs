using Application.DTOs;
using Application.EntityHandler.Queries.Interface;
using EntityHandler.Services.Interface;

namespace EntityHandler.Services
{
    public class FeaturedProductService : IFeaturedProductService
    {
        private readonly IFeaturedProductQueries _featuredProductQueries;

        public FeaturedProductService(IFeaturedProductQueries featuredProductQueries)
        {
            _featuredProductQueries = featuredProductQueries;
        }

        public async Task<IEnumerable<FeaturedProductDto>> GetFeaturedProductsAsync()
        {
            return await _featuredProductQueries.GetFeaturedProductsAsync();
        }
    }
}
