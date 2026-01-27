using Carter;
using Microsoft.AspNetCore.Authentication;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Endpoints;

public class AuthEndpoints : ICarterModule
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

        group.MapGet("/login/google/callback",
            async (HttpContext context) => await GoogleCallback(context))
          .WithName("GoogleCallback")
          .WithSummary("Google OAuth callback")
          .AllowAnonymous();

        group.MapPost("/logout",
            async (HttpContext context) => await Logout(context))
          .WithName("Logout")
          .WithSummary("Logout current user")
          .RequireAuthorization();

        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get current authenticated user")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static IResult LoginWithGoogle(
        HttpContext context,
        string? returnUrl = null)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl ?? $"{ApiConstants.ApiBasePath}/auth/login/google/callback"
        };

        return Results.Challenge(properties, new[] { AuthConstants.GoogleScheme });
    }

    private static async Task<IResult> GoogleCallback(HttpContext context)
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

    private static async Task<IResult> Logout(HttpContext context)
    {
        await context.SignOutAsync(AuthConstants.DefaultScheme);
        return Results.Ok(new { message = "Logged out successfully" });
    }

    private static IResult GetCurrentUser(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return Results.Unauthorized();
        }

        var userInfo = new
        {
            context.User.Identity.Name,
            Claims = context.User.Claims.Select(c => new { c.Type, c.Value })
        };

        return Results.Ok(userInfo);
    }
}

