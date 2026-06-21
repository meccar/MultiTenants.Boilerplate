using BuildingBlocks.Core.Seedwork.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stockroom.Infrastructure.Persistence.Repositories;

namespace Stockroom.Infrastructure.Configurations.Repositories;

public static class RepositoryConfiguration
{
    public static IServiceCollection AddRepositoryConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}