using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MultiTenants.Boilerplate.Endpoints.User.CreateUser;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Endpoints.User.CreateUser;

public class CreateUserEndpoint : ICarterModule
{
    [Obsolete("Obsolete")]
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup($"{ApiConstants.ApiBasePath}/users")
            .WithTags("Users")
            .WithOpenApi();

        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithSummary("Create a new user")
            .Produces<string>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> CreateUser(
        [FromBody] CreateUserCommand command,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(new { error = result.Error });
        }

        return Results.Created($"/api/users/{result.Value}", new { id = result.Value });
    }
}






