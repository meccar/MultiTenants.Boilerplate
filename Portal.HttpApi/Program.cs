using BuildingBlocks.Shared.Configuration;
using BuildingBlocks.Shared.Helpers;
using Carter;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Host.Configurations;
using Host.Middlewares;
using Host.Services;
using Identity.Application;
using Identity.Infrastructure;
using Identity.Domain;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using BuildingBlocks.Core.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddShared();
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(Identity.Application.AssemblyReference.Assembly));
builder.Services.ConfigureIdentityApplicationDependencyInjection(builder.Configuration);
builder.Services.ConfigureIdentityInfrastructureDependencyInjection(builder.Configuration);
builder.Services.ConfigureTenancyDomainDependencyInjection(builder.Configuration);
builder.Services.AddScoped<ITenantProvider, TenantProvider>();

builder.Services.AddHttpApi(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowConfiguredOrigins", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseGlobalExceptionHandling();

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
}

app.UseHttpsRedirection();
app.UseCors("AllowConfiguredOrigins");
app.UseRateLimiter();

app.UseMultiTenant();

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<TenantMiddleware>();

app.MapControllers();
app.MapCarter();

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
