using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands;

public record CreateUserCommand(
    string Email,
    string UserName,
    string? Password) : IRequest<Result<string>>;


