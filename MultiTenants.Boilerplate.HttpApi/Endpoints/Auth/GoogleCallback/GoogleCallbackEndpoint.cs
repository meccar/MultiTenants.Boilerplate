using Carter;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using MultiTenants.Boilerplate.Endpoints.Auth.GoogleCallback;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Endpoints.Auth.GoogleCallback;

public class GoogleCallbackEndpoint : ICarterModule
{
    [Obsolete("Obsolete")]
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup($"{ApiConstants.ApiBasePath}/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapGet("/login/google/callback",
            async (HttpContext context) => await HandleCallback(context))
          .WithName("GoogleCallback")
          .WithSummary("Google OAuth callback")
          .AllowAnonymous();
    }

    private static async Task<IResult> HandleCallback(HttpContext context)
    {
        var result = await context.AuthenticateAsync(AuthConstants.GoogleScheme);

        if (!result.Succeeded)
        {
            return Results.Unauthorized();
        }

        // Sign in the user with the default scheme
        await context.SignInAsync(AuthConstants.DefaultScheme, result.Principal!);

        return Results.Ok(new { message = "Authentication successful" });
    }
}

