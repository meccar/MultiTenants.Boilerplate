using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;

namespace MultiTenants.Boilerplate.Application.Configuration;

/// <summary>
/// Configuration extensions for caching services
/// </summary>
public static class CachingConfiguration
{
    /// <summary>
    /// Adds caching services (Redis or In-Memory) based on configuration
    /// </summary>
    public static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var cacheType = configuration["Caching:Type"]?.ToLowerInvariant();

        switch (cacheType)
        {
            case "redis":
                services.AddRedisCache(configuration);
                break;
            case "memory":
            default:
                services.AddMemoryCache();
                services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
                break;
        }

        return services;
    }

    private static IServiceCollection AddRedisCache(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
            ?? configuration["Caching:Redis:ConnectionString"]
            ?? "localhost:6379";

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
            options.InstanceName = configuration["Caching:Redis:InstanceName"] ?? "MultiTenants:";
        });

        // Also register IConnectionMultiplexer for advanced Redis operations
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            return ConnectionMultiplexer.Connect(connectionString);
        });

        return services;
    }
}
