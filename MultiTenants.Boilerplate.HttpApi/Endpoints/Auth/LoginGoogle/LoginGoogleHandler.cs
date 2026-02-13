using MediatR;
using Microsoft.AspNetCore.Authentication;
using MultiTenants.Boilerplate.Shared.Constants;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.LoginGoogle;

internal class LoginGoogleHandler : IRequestHandler<LoginGoogleCommand, Result<LoginGoogleResult>>
{
    public Task<Result<LoginGoogleResult>> Handle(LoginGoogleCommand request, CancellationToken cancellationToken)
    {
        var redirectUri = request.ReturnUrl ?? $"{ApiConstants.ApiBasePath}/auth/login/google/callback";
        var result = new LoginGoogleResult(redirectUri);
        return Task.FromResult(Result<LoginGoogleResult>.Success(result));
    }
}

