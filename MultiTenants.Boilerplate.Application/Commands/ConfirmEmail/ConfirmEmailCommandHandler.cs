using MediatR;
using Microsoft.Extensions.Logging;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.ConfirmEmail;

public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;

    public ConfirmEmailCommandHandler(
        IIdentityService identityService,
        ILogger<ConfirmEmailCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.ConfirmEmailAsync(request.UserId, request.Code, cancellationToken);

        if (result.IsFailure)
            _logger.LogWarning("ConfirmEmail failed for user {UserId}: {Error}", request.UserId, result.Error);
        else
            _logger.LogInformation("Email confirmed for user {UserId}.", request.UserId);

        return result;
    }
}
