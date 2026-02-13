using MediatR;
using Microsoft.AspNetCore.Identity;
using MultiTenants.Boilerplate.Application.DTOs;
using MultiTenants.Boilerplate.Shared.Utilities;
using Finbuckle.MultiTenant.Abstractions;

namespace MultiTenants.Boilerplate.Endpoints.User.GetUserById;

internal class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto?>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;

    public GetUserByIdHandler(
        UserManager<IdentityUser> userManager,
        IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor)
    {
        _userManager = userManager;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<Result<UserDto?>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var tenantContext = _tenantContextAccessor.MultiTenantContext;
        if (tenantContext.TenantInfo == null)
        {
            return Result<UserDto?>.Failure("Tenant context not found");
        }

        // Finbuckle's UserManager automatically filters by tenant context
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<UserDto?>.Success(null);
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            EmailConfirmed = user.EmailConfirmed
        };

        return Result<UserDto?>.Success(userDto);
    }
}






