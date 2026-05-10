using Microsoft.Extensions.Configuration;
    
namespace BuildingBlocks.Shared.Helpers;
public static class ConfigurationHelper
{
    /// <summary>
    /// Returns the named connection string or throws InvalidOperationException when the value is null, empty, or whitespace.
    /// </summary>
    public static string GetRequiredValue(
        this IConfiguration configuration, string name)
    {
        var value = configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(value))
            throw new InvalidOperationException(
                $"Configuration value for '{name}' is required but was not found.");
        return value;
    }
    
    public static T GetRequiredValue<T>(
        this IConfiguration configuration,
        string key)
    {
        var section = configuration.GetSection(key);

        if (!section.Exists())
            throw new InvalidOperationException(
                $"Configuration value for '{key}' is required but was not found.");

        return configuration.GetValue<T>(key)!;
    }
    
    public static T GetSection<T>(
        this IConfiguration configuration,
        string key)
        where T : class, new()
    {
        var section = configuration.GetSection(key);

        if (!section.Exists())
        {
            throw new InvalidOperationException(
                $"Configuration section '{key}' is missing.");
        }

        return section.Get<T>()
               ?? throw new InvalidOperationException(
                   $"Configuration section '{key}' is invalid.");
    }
}
