using MediatR;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Application.DTOs;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Queries.ManageAccount;

public class GetManageInfoQueryHandler : IRequestHandler<GetManageInfoQuery, Result<ManageInfoDto>>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<GetManageInfoQueryHandler> _logger;

    public GetManageInfoQueryHandler(
        IIdentityService identityService,
        ILogger<GetManageInfoQueryHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result<ManageInfoDto>> Handle(GetManageInfoQuery request, CancellationToken cancellationToken)
    {
        var info = await _identityService.GetManageInfoAsync(request.UserId, cancellationToken);
        if (info is null)
        {
            _logger.LogWarning("GetManageInfo: user {UserId} not found.", request.UserId);
            return Result<ManageInfoDto>.Failure("User not found.");
        }

        return Result<ManageInfoDto>.Success(info);
    }
}
