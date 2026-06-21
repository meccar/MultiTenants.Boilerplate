using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stockroom.Infrastructure.Configurations.Repositories;

namespace Stockroom.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureStockroomInfrastructureDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddRepositoryConfiguration(configuration);
        
        return services;
    }
}