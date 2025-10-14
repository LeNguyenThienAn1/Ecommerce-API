using Application.DTOs;

namespace EntityHandler.Queries.Interface
{
    public interface IChatQueries
    {
        Task<List<ProductDto>> SearchProductsAsync(string keyword);
    }
}
