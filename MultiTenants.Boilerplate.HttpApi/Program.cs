using Carter;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using MultiTenants.Boilerplate.Application.Configuration;
using MultiTenants.Boilerplate.Domain.Configuration;
using MultiTenants.Boilerplate.Infrastructure.Configuration;
using MultiTenants.Boilerplate.Shared.Configuration;
using MultiTenants.Boilerplate.Configurations;
using MultiTenants.Boilerplate.Middlewares;
using MultiTenants.Boilerplate.Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// Layer-by-Layer Dependency Injection
// ============================================
// Each project has its own DI registration method
// HttpApi (outer layer) orchestrates all layers

// 1. Shared Layer (foundation - no dependencies)
builder.Services.AddShared();

// 2. Domain Layer (depends on Shared)
builder.Services.AddDomain();

// 3. Application Layer (depends on Domain and Shared)
builder.Services.AddApplication(builder.Configuration);

// 4. Infrastructure Layer (Identity, EF Core, IIdentityService)
builder.Services.AddInfrastructure(builder.Configuration);

// 5. HttpApi Layer (depends on all layers)
builder.Services.AddHttpApi(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    var googleClientId = app.Configuration["Authentication:Google:ClientId"];
    var apiVersion = app.Configuration["Api:Version"]?.Trim() ?? "v1";
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"Multi-Tenant API {apiVersion}");
        // Only expose ClientId to Swagger UI (ClientId is public, ClientSecret should remain server-side only)
        // Swagger UI will use PKCE flow which doesn't require the client secret in the browser
        if (!string.IsNullOrWhiteSpace(googleClientId))
        {
            c.OAuthClientId(googleClientId);
        }
        // Note: ClientSecret is intentionally NOT passed to Swagger UI for security
        // The OAuth flow uses PKCE which doesn't require the secret in the client
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();
app.UseGlobalExceptionHandling();
app.UseCors("AllowConfiguredOrigins");
app.UseRateLimiter();

// Multi-tenant middleware must be before authentication
app.UseMultiTenant();

app.UseAuthentication();
app.UseAuthorization();

// Map Carter endpoints
app.MapCarter();

// Map health check endpoints
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("self")
});

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("ready") || r.Tags.Contains("self")
});

app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = r => r.Tags.Contains("self")
});

app.Run();
