using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Stockroom.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection ConfigureStockroomApplicationDependencyInjection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services;
    }
}