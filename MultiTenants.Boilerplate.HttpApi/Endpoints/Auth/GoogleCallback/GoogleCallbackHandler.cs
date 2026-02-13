using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MultiTenants.Boilerplate.Shared.Constants;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.GoogleCallback;

internal class GoogleCallbackHandler : IRequestHandler<GoogleCallbackCommand, Result<GoogleCallbackResult>>
{
    private readonly HttpContext _httpContext;

    public GoogleCallbackHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available");
    }

    public async Task<Result<GoogleCallbackResult>> Handle(GoogleCallbackCommand request, CancellationToken cancellationToken)
    {
        var result = await _httpContext.AuthenticateAsync(AuthConstants.GoogleScheme);

        if (!result.Succeeded)
        {
            return Result<GoogleCallbackResult>.Failure("Authentication failed");
        }

        // Sign in the user with the default scheme
        await _httpContext.SignInAsync(AuthConstants.DefaultScheme, result.Principal!);

        return Result<GoogleCallbackResult>.Success(new GoogleCallbackResult("Authentication successful"));
    }
}

