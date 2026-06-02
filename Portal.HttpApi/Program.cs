using BuildingBlocks.Shared.Helpers;
using Carter;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Host.Configurations;
using Host.Middlewares;
using Identity.Application;
using Identity.Infrastructure;
using Identity.Domain;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Host;
using Identity.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHostDependencyInjection(builder.Configuration);
builder.Services.ConfigureIdentityApplicationDependencyInjection(builder.Configuration);
builder.Services.ConfigureIdentityInfrastructureDependencyInjection(builder.Configuration);
builder.Services.ConfigureTenancyDomainDependencyInjection(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var googleClientId = app.Configuration.GetRequiredValue(
                    "Authentication:Google:ClientId");
    var apiOptions = app.Configuration.GetSection<ApiOptions>("Api");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            $"/swagger/{apiOptions.Version}/swagger.json",
            $"Multi-Tenant API {apiOptions.Version}");
        if (!string.IsNullOrWhiteSpace(googleClientId))
            c.OAuthClientId(googleClientId);
        c.OAuthUsePkce();
    });
    app.UseHttpsRedirection();
}
app.UseGlobalExceptionHandling();
app.UseRateLimiter();
app.UseCors("AllowConfiguredOrigins");
app.UseMultiTenant();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapCarter();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<AppDbSeeder>();
    await seeder.SeedAsync();
}

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = r => 
        r.Tags.Contains("self")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = r => 
        r.Tags.Contains("ready") || r.Tags.Contains("self")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = r => 
        r.Tags.Contains("self")
});

app.Run();
