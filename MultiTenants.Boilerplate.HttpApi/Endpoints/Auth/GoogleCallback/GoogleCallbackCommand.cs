using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.GoogleCallback;

public record GoogleCallbackCommand : IRequest<Result<GoogleCallbackResult>>;

public record GoogleCallbackResult(string Message);

