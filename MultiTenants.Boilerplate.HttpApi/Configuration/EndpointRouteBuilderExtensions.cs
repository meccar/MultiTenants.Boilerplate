using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MultiTenants.Boilerplate.Configurations;

/// <summary>
/// Extension methods to resolve API base path from configuration when registering endpoints.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Gets the configured API base path (e.g. "/api/v1") from ApiOptions.
    /// </summary>
    public static string GetApiBasePath(this IEndpointRouteBuilder app)
    {
        var options = app.ServiceProvider.GetRequiredService<IOptions<ApiOptions>>().Value;
        return options.BasePath;
    }
}
