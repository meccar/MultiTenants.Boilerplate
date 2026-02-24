using MediatR;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IIdentityService identityService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ResetPasswordAsync(
            request.Email, request.Token, request.NewPassword, cancellationToken);

        if (result.IsFailure)
            _logger.LogWarning("ResetPassword failed for {Email}: {Error}", request.Email, result.Error);
        else
            _logger.LogInformation("Password reset successfully for {Email}.", request.Email);

        return result;
    }
}
