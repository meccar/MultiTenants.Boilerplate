using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.Logout;

public record LogoutCommand : IRequest<Result<LogoutResult>>;

public record LogoutResult(string Message);

