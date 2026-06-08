using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tenancy.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureTenancyInfrastructureDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            
            return services;
        }
    }
}
