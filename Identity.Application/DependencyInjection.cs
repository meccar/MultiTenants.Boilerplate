using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureIdentityApplicationDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            return services;
        }
    }
}
