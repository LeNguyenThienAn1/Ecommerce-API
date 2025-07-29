using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace EF.PostgreSQL.Actions
{
    public class AddServicesActions
    {
        public void Execute(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            }

            services.AddEntityFramework.AddDbContext<EcommerceDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });
        }
    }
}