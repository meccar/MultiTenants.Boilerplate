using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Queries.GetCurrentUser;

public record GetCurrentUserQuery(string token) : IRequest<Result<UserDto?>>;


