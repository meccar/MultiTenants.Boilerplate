using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MultiTenants.Boilerplate.Shared.Utilities;
using Finbuckle.MultiTenant.Abstractions;

namespace MultiTenants.Boilerplate.Endpoints.User.CreateUser;

internal class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<string>>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateUserHandler(
        UserManager<IdentityUser> userManager,
        IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _tenantContextAccessor = tenantContextAccessor;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var tenantContext = _tenantContextAccessor.MultiTenantContext;
        if (tenantContext.TenantInfo == null)
        {
            return Result<string>.Failure("Tenant context not found");
        }

        var user = new IdentityUser
        {
            Email = request.Email,
            UserName = request.UserName
        };
        // Finbuckle automatically handles tenant isolation - no need to set TenantId manually

        IdentityResult result;
        if (!string.IsNullOrEmpty(request.Password))
        {
            result = await _userManager.CreateAsync(user, request.Password);
        }
        else
        {
            result = await _userManager.CreateAsync(user);
        }

        if (result.Succeeded)
        {
            return Result<string>.Success(user.Id);
        }

        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        return Result<string>.Failure(errors);
    }
}






