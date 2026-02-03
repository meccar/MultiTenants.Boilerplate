namespace MultiTenants.Boilerplate.Configurations;
public static class CorsConfiguration
{
    public static IServiceCollection AddCorsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration
    ) {
        var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? ["http://localhost:3000", "http://localhost:5173"];
        services.AddCors(options =>
        {
            options.AddPolicy("AllowConfiguredOrigins", builder =>
            {
                builder
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition", "X-Total-Count");
            });
            options.AddPolicy("RetrictedCors", builder =>
            {
                builder
                    .WithOrigins(allowedOrigins)
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Content-Type", "Authorization")
                    .AllowCredentials();
            });
        });

        return services;
    }
}
