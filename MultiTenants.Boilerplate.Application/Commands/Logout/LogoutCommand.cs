using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.Logout;

public record LogoutCommand : IRequest<Result>;
