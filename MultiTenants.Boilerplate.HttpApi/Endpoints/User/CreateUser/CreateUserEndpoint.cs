using Carter;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MultiTenants.Boilerplate.Application.Commands;
using MultiTenants.Boilerplate.Configurations;

namespace MultiTenants.Boilerplate.Endpoints.User.CreateUser;

public class CreateUserEndpoint : ICarterModule
{
    [Obsolete("Obsolete")]
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var basePath = app.GetApiBasePath();
        var group = app.MapGroup($"{basePath}/users")
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
        IOptions<ApiOptions> apiOptions,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(new { error = result.Error });
        }

        var basePath = apiOptions.Value.BasePath;
        return Results.Created($"{basePath}/users/{result.Value}", new { id = result.Value });
    }
}






