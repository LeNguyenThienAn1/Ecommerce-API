using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace EF.PostgreSQL
{
    public class EcommerceDbDesignTimeFactory : IDesignTimeDbContextFactory<EcommerceDbContext>
    {
        public EcommerceDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EcommerceDbContext>();
            builder.UseNpgsql();
            var services = new ServiceCollection();
            services.AddScoped<IEcommerceMapper, NpgSqlEcommerceModelMapper>();
            builder.UseApplicationServiceProvider(services.BuildServiceProvider());
            return new EcommerceDbContext(builder.Options);
        }
    }
}
