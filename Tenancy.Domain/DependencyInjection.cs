using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenancy.Domain.Interfaces;
using Tenancy.Domain.Models;

namespace Identity.Domain
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection ConfigureTenancyDomainDependencyInjection(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddScoped<ITenant, TenantContext>();

            return services;
        }
    }
}
