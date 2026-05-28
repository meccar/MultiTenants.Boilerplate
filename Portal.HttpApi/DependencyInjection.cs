using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Shared.Configuration;
using Host.Configurations;
using Host.Services;
using Identity.Infrastructure.Persistence.Data;

namespace Host
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureHostDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddShared();
            services.AddMediatR(cf =>
                cf.RegisterServicesFromAssembly(
                    Identity.Application.AssemblyReference.Assembly));
            services.AddScoped<AppDbSeeder>();
            services.AddScoped<ITenantProvider, TenantProvider>();
            services.AddHttpApi(configuration);
            
            return services;
        }
    }
}
