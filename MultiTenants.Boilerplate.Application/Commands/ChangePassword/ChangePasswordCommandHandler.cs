using MediatR;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IIdentityService identityService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ChangePasswordAsync(
            request.UserId, request.CurrentPassword, request.NewPassword, cancellationToken);

        if (result.IsFailure)
            _logger.LogWarning("ChangePassword failed for user {UserId}: {Error}", request.UserId, result.Error);
        else
            _logger.LogInformation("Password changed for user {UserId}.", request.UserId);

        return result;
    }
}
