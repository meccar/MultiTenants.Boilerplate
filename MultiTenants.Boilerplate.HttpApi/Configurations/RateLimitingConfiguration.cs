using System.Threading.RateLimiting;

namespace MultiTenants.Boilerplate.Configurations;
public static class RateLimitingConfiguration
{
    public static IServiceCollection AddRateLimitingConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    ) {
        var rateLimitConfig = configuration.GetSection("RateLimiting");
        var permitLimit = rateLimitConfig.GetValue<int>("PermitLimit", 100);
        var windowSeconds = rateLimitConfig.GetValue<int>("WindowSizeSeconds", 60);
        var queueLimit = rateLimitConfig.GetValue<int>("QueueLimit", 2);
        var segmentsPerWindow = rateLimitConfig.GetValue<int>("SegmentsPerWindow", 2);

        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
            {
                return RateLimitPartition.GetSlidingWindowLimiter(
                    partitionKey: ctx.User.Identity?.Name 
                        ?? ctx.Connection.RemoteIpAddress?.ToString()
                        ?? "anonymous",
                    factory: partition => new SlidingWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = permitLimit,
                        QueueLimit = queueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        Window = TimeSpan.FromSeconds(windowSeconds),
                        SegmentsPerWindow = segmentsPerWindow,
                    });
            });
            options.OnRejected = async (ctx, cancellationToken) =>
            {
                ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                ctx.HttpContext.Response.ContentType = "application/json";

                var response = new
                {
                    Message = "Too many requests. Please try again later."
                };
                await ctx.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            };
        });

        return services;
    }
}
