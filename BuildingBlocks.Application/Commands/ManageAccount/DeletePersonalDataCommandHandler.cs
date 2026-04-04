using MediatR;
using Microsoft.Extensions.Logging;
using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.ManageAccount;

public class DeletePersonalDataCommandHandler : IRequestHandler<DeletePersonalDataCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ILogger<DeletePersonalDataCommandHandler> _logger;

    public DeletePersonalDataCommandHandler(
        IIdentityService identityService,
        ILogger<DeletePersonalDataCommandHandler> logger)
    {
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<Result> Handle(DeletePersonalDataCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.DeletePersonalDataAsync(
            request.UserId, request.Password, cancellationToken);

        if (result.IsFailure)
            _logger.LogWarning("DeletePersonalData failed for user {UserId}: {Error}", request.UserId, result.Error);
        else
            _logger.LogInformation("User {UserId} deleted their account.", request.UserId);

        return result;
    }
}
