using MediatR;
using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Application.Commands.CreateUserAccount;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.Register;

public class CreateUserAccountCommandHandler : IRequestHandler<CreateUserAccountCommand, Result<string>>
{
    private readonly IIdentityService _identityService;

    public CreateUserAccountCommandHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<string>> Handle(CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.CreateUserAsync(
            request.Email,
            request.Password,
            request.TenantName,
            cancellationToken);
    }
}
