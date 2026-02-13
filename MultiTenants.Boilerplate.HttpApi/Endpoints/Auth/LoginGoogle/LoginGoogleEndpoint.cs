using Carter;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using MultiTenants.Boilerplate.Endpoints.Auth.LoginGoogle;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Endpoints.Auth.LoginGoogle;

public class LoginGoogleEndpoint : ICarterModule
{
    [Obsolete("Obsolete")]
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup($"{ApiConstants.ApiBasePath}/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapGet("/login/google", LoginWithGoogle)
            .WithName("LoginWithGoogle")
            .WithSummary("Initiate Google OAuth login")
            .AllowAnonymous();
    }

    private static IResult LoginWithGoogle(
        HttpContext context,
        string? returnUrl = null)
    {
        var request = context.Request;
        var baseUrl = $"{request.Scheme}://{request.Host}";
        var defaultCallbackUrl = $"{baseUrl}{ApiConstants.ApiBasePath}/auth/login/google/callback";
        
        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl ?? defaultCallbackUrl
        };

        return Results.Challenge(properties, new[] { AuthConstants.GoogleScheme });
    }
}

