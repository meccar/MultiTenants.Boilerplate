using BuildingBlocks.Core.Abstractions;
using Host.Services;

namespace Host
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureHostDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddScoped<ITenantProvider, TenantProvider>();
            return services;
        }
    }
}
