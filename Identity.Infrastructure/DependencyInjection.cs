using Identity.Infrastructure.Configurations.Auth;
using Identity.Infrastructure.Configurations.Jobs;
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
            services.AddIdentityConfiguration(configuration);
            services.AddJwtConfiguration(configuration);
            services.AddRepositoryConfiguration(configuration);
            services.AddQuartzInfrastructure();
            
            return services;
        }
    }
}
