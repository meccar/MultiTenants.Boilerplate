using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Stockroom.Domain;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureStockroomDomainDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }
}