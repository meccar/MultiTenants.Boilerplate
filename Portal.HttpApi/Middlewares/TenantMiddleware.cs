using Tenancy.Domain.Interfaces;
using Tenancy.Domain.Models;

namespace BuildingBlocks.Middlewares
{
    public class TenantMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantMiddleware> _logger;

        public TenantMiddleware(
            RequestDelegate next, ILogger<TenantMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync (
            HttpContext context, ITenant tenantContext)
        {
            const string tenantHeaderName = "X-Tenant-Name";

            if (context.Request.Headers.TryGetValue(
                tenantHeaderName, out var tenantValues))
            {
                var tenantName = tenantValues.FirstOrDefault();

                if (string.IsNullOrWhiteSpace(tenantName))
                {
                    _logger.LogWarning("Tenant header '{TenantHeader}' is empty", tenantHeaderName);
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsJsonAsync(new { error = "Tenant name cannot be empty" });
                    return;
                }

                // TODO: Validate tenant exists in your database
                // var tenantExists = await _tenantRepository.ExistsAsync(tenantName);
                // if (!tenantExists)
                // {
                //     _logger.LogWarning("Tenant '{TenantName}' not found", tenantName);
                //     context.Response.StatusCode = StatusCodes.Status403Forbidden;
                //     await context.Response.WriteAsJsonAsync(new { error = "Tenant not found" });
                //     return;
                // }

                ((TenantContext)tenantContext).TenantName = tenantName;
                ((TenantContext)tenantContext).IsMultiTenant = true;

                _logger.LogInformation("Tenant '{TenantName}' resolved from header", tenantName);
            }
            else
            {
                _logger.LogWarning("Required tenant header '{TenantHeader}' is missing", tenantHeaderName);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Missing required header: X-Tenant-Name" });
                return;
            }

            await _next(context);
        }
    }
}
