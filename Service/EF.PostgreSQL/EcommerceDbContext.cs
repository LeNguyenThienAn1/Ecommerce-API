using Microsoft.EntityFrameworkCore;

namespace EF.PostgreSQL
{
    public class EcommerceDbContext : EcommerceDbContextBase, IEcommerceDbContext
    {
        public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var mapper = this.GetService<IEcommerceMapper>();
            if (mapper == null)
            {
                mapper = new NpgSqlCalendarModelMapper();
            }
        }
    }
}
