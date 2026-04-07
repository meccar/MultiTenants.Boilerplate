using MediatR;
using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Queries.GetCurrentUser;

public record GetUserByIdQuery(string UserId) : IRequest<Result<UserDto?>>;


