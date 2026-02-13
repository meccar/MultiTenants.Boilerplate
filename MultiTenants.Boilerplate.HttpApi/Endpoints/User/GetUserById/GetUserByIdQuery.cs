using MediatR;
using MultiTenants.Boilerplate.Application.DTOs;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.User.GetUserById;

public record GetUserByIdQuery(string UserId) : IRequest<Result<UserDto?>>;






