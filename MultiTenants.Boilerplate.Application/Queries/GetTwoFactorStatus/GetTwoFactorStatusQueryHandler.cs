using MediatR;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Queries.GetTwoFactorStatus;

public class GetTwoFactorStatusQueryHandler : IRequestHandler<GetTwoFactorStatusQuery, Result<bool>>
{
    private readonly ILogger<GetTwoFactorStatusQueryHandler> _logger;

    public GetTwoFactorStatusQueryHandler(ILogger<GetTwoFactorStatusQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<bool>> Handle(GetTwoFactorStatusQuery request, CancellationToken cancellationToken)
    {
        // Two-factor authentication requires IUserTwoFactorStore.
        // This store is not yet implemented; returning false as a stub.
        _logger.LogDebug("GetTwoFactorStatus queried for user {UserId} — 2FA not yet supported.", request.UserId);
        return Task.FromResult(Result<bool>.Success(false));
    }
}
