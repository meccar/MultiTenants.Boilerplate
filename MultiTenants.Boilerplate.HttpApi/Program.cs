using Carter;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Microsoft.EntityFrameworkCore;
using MultiTenants.Boilerplate.Application.Configuration;
using MultiTenants.Boilerplate.Domain.Configuration;
using MultiTenants.Boilerplate.Infrastructure.Configuration;
using MultiTenants.Boilerplate.Infrastructure.Data;
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

// Ensure database exists and apply pending migrations (creates/updates MultiTenantsIdentity)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed. Ensure SQL Server/LocalDB is running and the connection string is correct.");
        throw;
    }
}

// Seed default tenant (TAF), Admin role, and admin user (idempotent).
// Set tenant context *before* resolving DataSeeder so DbContext is created with correct CurrentTenantId.
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var tenantStore = scope.ServiceProvider.GetRequiredService<Finbuckle.MultiTenant.Abstractions.IMultiTenantStore<Finbuckle.MultiTenant.Abstractions.TenantInfo>>();
        var tenantSetter = scope.ServiceProvider.GetRequiredService<Finbuckle.MultiTenant.Abstractions.IMultiTenantContextSetter>();
        var tenant = await tenantStore.GetByIdentifierAsync("taf");
        if (tenant == null)
        {
            tenant = new Finbuckle.MultiTenant.Abstractions.TenantInfo
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = "taf",
                Name = "TAF"
            };
            await tenantStore.AddAsync(tenant);
        }
        tenantSetter.MultiTenantContext = new Finbuckle.MultiTenant.Abstractions.MultiTenantContext<Finbuckle.MultiTenant.Abstractions.TenantInfo>(tenant);

        var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Data seeding failed.");
        throw;
    }
}

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
