using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Queries.GetCurrentUser;

public class GetUserByIdQueryHandler 
    : IRequestHandler<GetUserByIdQuery, Result<UserDto?>>
{
    private readonly IIdentityService _identityService;
    private readonly ITenantProvider _tenantProvider;

    public GetUserByIdQueryHandler(
        IIdentityService identityService,
        ITenantProvider tenantProvider)
    {
        _identityService = identityService;
        _tenantProvider = tenantProvider;
    }

    public async Task<Result<UserDto?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (_tenantProvider.GetCurrentTenantId() == null)
            return Result<UserDto?>.Failure("Tenant context not found");

        var userDto = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
        return Result<UserDto?>.Success(userDto);
    }
}
