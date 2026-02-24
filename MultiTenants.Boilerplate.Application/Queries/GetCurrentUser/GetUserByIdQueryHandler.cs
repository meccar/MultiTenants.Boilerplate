using MediatR;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Application.DTOs;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Queries.GetCurrentUser;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto?>>
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

        var user = await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result<UserDto?>.Success(null);

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt ?? DateTime.MinValue
        };

        return Result<UserDto?>.Success(userDto);
    }
}
