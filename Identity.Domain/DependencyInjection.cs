using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureIdentityDomainDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services;
        }
    }
}
