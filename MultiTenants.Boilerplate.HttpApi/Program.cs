using Carter;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.OpenApi;
using MultiTenants.Boilerplate.Application.Configuration;
using MultiTenants.Boilerplate.Shared.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Multi-Tenant API",
        Version = ApiConstants.ApiVersion,
        Description = "Multi-tenant API with CQRS, MongoDB, and OAuth"
    });

    // Add security definition for OAuth
    // OAuth URLs can be overridden via configuration: Authentication:Google:AuthorizationUrl and Authentication:Google:TokenUrl
    var authorizationUrl = builder.Configuration["Authentication:Google:AuthorizationUrl"] 
        ?? AuthConstants.GoogleAuthorizationUrl;
    var tokenUrl = builder.Configuration["Authentication:Google:TokenUrl"] 
        ?? AuthConstants.GoogleTokenUrl;
    
    c.AddSecurityDefinition("Google", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(authorizationUrl),
                TokenUrl = new Uri(tokenUrl),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID" },
                    { "profile", "Profile" },
                    { "email", "Email" }
                }
            }
        }
    });
});

builder.Services.AddApplication(builder.Configuration);

// Retrieve and validate Google OAuth credentials
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];

// Validate ClientId - fail fast with clear error message
if (string.IsNullOrWhiteSpace(googleClientId) || 
    googleClientId.Equals("YOUR_GOOGLE_CLIENT_ID", StringComparison.OrdinalIgnoreCase))
{
    var envVarName = "Authentication__Google__ClientId";
    throw new InvalidOperationException(
        $"Google OAuth ClientId is required but not configured. " +
        $"Please set the environment variable '{envVarName}' or configure it via User Secrets. " +
        $"For development, use: dotnet user-secrets set \"Authentication:Google:ClientId\" \"your-client-id\"");
}

// Validate ClientSecret - fail fast with clear error message
if (string.IsNullOrWhiteSpace(googleClientSecret) || 
    googleClientSecret.Equals("YOUR_GOOGLE_CLIENT_SECRET", StringComparison.OrdinalIgnoreCase))
{
    var envVarName = "Authentication__Google__ClientSecret";
    throw new InvalidOperationException(
        $"Google OAuth ClientSecret is required but not configured. " +
        $"Please set the environment variable '{envVarName}' or configure it via User Secrets. " +
        $"For development, use: dotnet user-secrets set \"Authentication:Google:ClientSecret\" \"your-client-secret\"");
}

builder.Services.AddAuthentication()
    .AddGoogle(AuthConstants.GoogleScheme, options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.SignInScheme = AuthConstants.DefaultScheme;
        options.CallbackPath = $"{ApiConstants.ApiBasePath}/auth/login/google/callback";
    });

builder.Services.AddMultiTenant<TenantInfo>()
    .WithRouteStrategy()
    .WithInMemoryStore();

// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add Carter for endpoint routing
builder.Services.AddCarter();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Multi-Tenant API v1");
        // Only expose ClientId to Swagger UI (ClientId is public, ClientSecret should remain server-side only)
        // Swagger UI will use PKCE flow which doesn't require the client secret in the browser
        c.OAuthClientId(googleClientId);
        // Note: ClientSecret is intentionally NOT passed to Swagger UI for security
        // The OAuth flow uses PKCE which doesn't require the secret in the client
        c.OAuthUsePkce();
    });
}

app.UseHttpsRedirection();

// Multi-tenant middleware must be before authentication
app.UseMultiTenant();

app.UseAuthentication();
app.UseAuthorization();

// Map Carter endpoints
app.MapCarter();

app.Run();
