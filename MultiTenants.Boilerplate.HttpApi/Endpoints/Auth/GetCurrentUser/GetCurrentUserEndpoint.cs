using Carter;
using MediatR;
using MultiTenants.Boilerplate.Endpoints.Auth.GetCurrentUser;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Endpoints.Auth.GetCurrentUser;

public class GetCurrentUserEndpoint : ICarterModule
{
    [Obsolete("Obsolete")]
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup($"{ApiConstants.ApiBasePath}/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get current authenticated user")
            .RequireAuthorization()
            .Produces<GetCurrentUserResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);
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

