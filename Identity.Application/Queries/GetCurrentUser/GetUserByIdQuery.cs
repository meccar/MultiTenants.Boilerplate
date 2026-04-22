using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Queries.GetCurrentUser;

public record GetUserByIdQuery(string UserId) : IRequest<Result<UserDto?>>;


