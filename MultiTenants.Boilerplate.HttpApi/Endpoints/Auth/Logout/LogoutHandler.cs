using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MultiTenants.Boilerplate.Shared.Constants;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.Logout;

internal class LogoutHandler : IRequestHandler<LogoutCommand, Result<LogoutResult>>
{
    private readonly HttpContext _httpContext;

    public LogoutHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available");
    }

    public async Task<Result<LogoutResult>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _httpContext.SignOutAsync(AuthConstants.DefaultScheme);
        return Result<LogoutResult>.Success(new LogoutResult("Logged out successfully"));
    }
}

