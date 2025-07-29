using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Queries;
using EF.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EF.Actions
{
    public class AddServiceAction 
    {
        public int Priority => 300;

        public void Excute(IServiceCollection services, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            services.AddScoped<IProductQueries, ProductQueries>();
        }
    }
}
