using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MultiTenants.Boilerplate.Configurations;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.LoginGoogle;

internal class LoginGoogleHandler : IRequestHandler<LoginGoogleCommand, Result<LoginGoogleResult>>
{
    private readonly ApiOptions _apiOptions;

    public LoginGoogleHandler(IOptions<ApiOptions> apiOptions)
    {
        _apiOptions = apiOptions.Value;
    }

    public Task<Result<LoginGoogleResult>> Handle(LoginGoogleCommand request, CancellationToken cancellationToken)
    {
        var redirectUri = request.ReturnUrl ?? $"{_apiOptions.BasePath}/auth/login/google/callback";
        var result = new LoginGoogleResult(redirectUri);
        return Task.FromResult(Result<LoginGoogleResult>.Success(result));
    }
}

