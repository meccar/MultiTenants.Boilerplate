using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Stockroom.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureStockroomInfrastructureDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }
}