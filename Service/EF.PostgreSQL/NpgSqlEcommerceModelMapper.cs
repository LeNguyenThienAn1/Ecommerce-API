using EF;
using Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EF.PostgreSQL
{
    public class NpgSqlEcommerceModelMapper : IEcommerceMapper
    {
        public void ProductMap(EntityTypeBuilder<ProductEntity> entity)
        {
            entity.ToTable(EcommerceTableNames.TablePrefix + EcommerceTableNames.ProductTableName).HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnType("uuid").IsRequired();
        }
    }
}
