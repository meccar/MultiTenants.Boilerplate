using Carter;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using MultiTenants.Boilerplate.Endpoints.Auth.Logout;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Endpoints.Auth.Logout;

public class LogoutEndpoint : ICarterModule
{
    [Obsolete("Obsolete")]
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup($"{ApiConstants.ApiBasePath}/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/logout",
            async (HttpContext context) => await Logout(context))
          .WithName("Logout")
          .WithSummary("Logout current user")
          .RequireAuthorization();
    }

    private static async Task<IResult> Logout(HttpContext context)
    {
        await context.SignOutAsync(AuthConstants.DefaultScheme);
        return Results.Ok(new { message = "Logged out successfully" });
    }
}

