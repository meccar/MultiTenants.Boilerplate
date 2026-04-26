using BuildingBlocks.Shared.Utilities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Queries.GetTwoFactorStatus;

public class GetTwoFactorStatusQueryHandler 
    : IRequestHandler<GetTwoFactorStatusQuery, Result<bool>>
{
    private readonly ILogger<GetTwoFactorStatusQueryHandler> _logger;

    public GetTwoFactorStatusQueryHandler(ILogger<GetTwoFactorStatusQueryHandler> logger)
    {
        _logger = logger;
    }

    public Task<Result<bool>> Handle(GetTwoFactorStatusQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetTwoFactorStatus queried for user {UserId} — 2FA not yet supported.", request.UserId);
        return Task.FromResult(Result<bool>.Success(false));
    }
}
