using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MultiTenants.Boilerplate.Application.Helpers;

namespace MultiTenants.Boilerplate.Application.Configuration;

/// <summary>
/// Configuration for MongoDB services
/// </summary>
public static class MongoDbConfiguration
{
    /// <summary>
    /// Adds MongoDB services to the service collection
    /// </summary>
    public static IServiceCollection AddMongoDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var mongoConnectionString = configuration.GetRequiredConfigurationValue("MongoDB");
        var mongoClient = new MongoClient(mongoConnectionString);
        var databaseName = configuration.GetValue<string>("MongoDB:DatabaseName") ?? "multitenants";

        services.AddSingleton<IMongoClient>(mongoClient);
        services.AddScoped(_ => mongoClient.GetDatabase(databaseName));

        return services;
    }
}
