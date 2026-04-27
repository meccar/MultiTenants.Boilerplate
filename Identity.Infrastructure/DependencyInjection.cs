using Identity.Infrastructure.Configurations.Repository;
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
            services.AddRepositoryConfiguration(configuration);
            
            
            return services;
        }
    }
}
