using Microsoft.Extensions.Configuration;

namespace MultiTenants.Boilerplate.Application.Helpers;
public static class GetConfigurationHelper
{
    /// <summary>
    /// Returns the named connection string or throws InvalidOperationException when the value is null, empty, or whitespace.
    /// </summary>
    public static string GetRequiredConfigurationValue(this IConfiguration configuration, string name)
    {
        var value = configuration.GetConnectionString(name);
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException(
                $"Configuration value for '{name}' is required but was not found.");
        }
        return value;
    }

    /// <summary>
    /// Returns the configuration value for the provided key or throws InvalidOperationException when the value is null, empty, or whitespace.
    /// </summary>
    public static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrEmpty(value))
        {
            var envVar = key.Replace(":", "_");
            throw new InvalidOperationException(
                $"Configuration value for '{key}' is required but was not found.");
        }
        return value;
    }
}
