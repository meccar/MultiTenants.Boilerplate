using Carter;
using MediatR;
using MultiTenants.Boilerplate.Endpoints.User.GetUserById;
using MultiTenants.Boilerplate.Shared.Constants;

namespace MultiTenants.Boilerplate.Endpoints.User.GetUserById;

public class GetUserByIdEndpoint : ICarterModule
{
    [Obsolete("Obsolete")]
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup($"{ApiConstants.ApiBasePath}/users")
            .WithTags("Users")
            .WithOpenApi();

        group.MapGet("/{id}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .Produces<Application.DTOs.UserDto>()
            .Produces(StatusCodes.Status404NotFound);
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






