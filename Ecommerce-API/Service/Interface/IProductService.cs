using Ecommerce_API.DTOs;
using Ecommerce_API.Model.Entity;

namespace Ecommerce_API
{
    public interface IProductService
    {
        public ProductDetailsDto GetProductInformationByIdAsync(Guid id);
    }
}
