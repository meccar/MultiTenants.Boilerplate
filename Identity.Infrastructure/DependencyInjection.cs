using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureIdentityInfrastructureDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services;
        }
    }
}
