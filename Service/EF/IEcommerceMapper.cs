using Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF
{
    public interface IEcommerceMapper
    {
        void ProductMap(EntityTypeBuilder<ProductEntity> entity);
    }
}
