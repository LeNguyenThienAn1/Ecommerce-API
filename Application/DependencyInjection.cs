using Microsoft.Extensions.DependencyInjection;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure;
using Infrastructure.Queries;
using Application.Interfaces.Queries; // namespace của CategoryQueries

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Đăng ký Queries
            services.AddScoped<ICategoryQueries, CategoryQueries>();

            // Đăng ký Services
            services.AddScoped<ICategoryService, CategoryService>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductQueries, ProductQueries>();

            return services;
        }
    }
}
