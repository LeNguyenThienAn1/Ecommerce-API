using Ecommerce_API.Model.Entity;

namespace Ecommerce_API.IQueries
{
    public interface IProductQuery
    {
        public ProductEntity GetProductInfomrationById(Guid id);
    }
}
