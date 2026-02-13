using MediatR;
using Microsoft.AspNetCore.Http;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.GetCurrentUser;

internal class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserResult>>
{
    private readonly HttpContext _httpContext;

    public GetCurrentUserHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available");
    }

    public Task<Result<GetCurrentUserResult>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (_httpContext.User.Identity?.IsAuthenticated != true)
        {
            return Task.FromResult(Result<GetCurrentUserResult>.Failure("User is not authenticated"));
        }

        var result = new GetCurrentUserResult(
            _httpContext.User.Identity.Name,
            _httpContext.User.Claims.Select(c => new ClaimInfo(c.Type, c.Value)));

        return Task.FromResult(Result<GetCurrentUserResult>.Success(result));
    }
}

