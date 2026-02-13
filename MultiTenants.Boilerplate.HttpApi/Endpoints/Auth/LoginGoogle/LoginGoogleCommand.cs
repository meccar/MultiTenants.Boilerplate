using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.LoginGoogle;

public record LoginGoogleCommand(string? ReturnUrl = null) : IRequest<Result<LoginGoogleResult>>;

public record LoginGoogleResult(string RedirectUri);

