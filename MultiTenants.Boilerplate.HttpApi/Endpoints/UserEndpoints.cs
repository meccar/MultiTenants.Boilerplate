using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MultiTenants.Boilerplate.Application.Commands;
using MultiTenants.Boilerplate.Application.Queries;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Endpoints;

public class UserEndpoints : ICarterModule
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

        group.MapGet("/{id}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .Produces<Application.DTOs.UserDto>()
            .Produces(StatusCodes.Status404NotFound);
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

    private static async Task<IResult> GetUserById(
        string id,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(new { error = result.Error });
        }

        if (result.Value == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(result.Value);
    }
}

