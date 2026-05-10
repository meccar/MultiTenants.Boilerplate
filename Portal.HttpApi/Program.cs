using BuildingBlocks.Middlewares;
using BuildingBlocks.Shared.Configuration;
using BuildingBlocks.Shared.Helpers;
using Carter;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Host.Configurations;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Services.AddShared();

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

if (app.Environment.IsDevelopment())
{
    
    var googleClientId = ConfigurationHelper.GetRequiredValue(
                    app.Configuration,"Authentication:Google:ClientId");
    var apiVersion = ConfigurationHelper.GetRequiredValue(
                    app.Configuration, "Api:Version");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(
            $"/swagger/{apiVersion}/swagger.json",
            $"Multi-Tenant API {apiVersion}");
        if (!string.IsNullOrWhiteSpace(googleClientId))
            c.OAuthClientId(googleClientId);
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();
app.UseGlobalExceptionHandling();
app.UseCors("AllowConfiguredOrigins");
app.UseRateLimiter();

app.UseMultiTenant();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();

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
