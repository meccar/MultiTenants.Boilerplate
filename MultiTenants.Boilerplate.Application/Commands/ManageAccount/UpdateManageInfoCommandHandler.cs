using MediatR;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ManageAccount;

public class UpdateManageInfoCommandHandler : IRequestHandler<UpdateManageInfoCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<UpdateManageInfoCommandHandler> _logger;

    public UpdateManageInfoCommandHandler(
        IIdentityService identityService,
        ILogger<UpdateManageInfoCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdateManageInfoCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateManageInfoAsync(
            request.UserId, request.Email, request.UserName, cancellationToken);

        if (result.IsFailure)
            _logger.LogWarning("UpdateManageInfo failed for user {UserId}: {Error}", request.UserId, result.Error);
        else
            _logger.LogInformation("Account info updated for user {UserId}.", request.UserId);

        return result;
    }
}
