using Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tenancy.Domain.Interfaces;

namespace Identity.Application.Commands.CreateUserAccount;

public class CreateUserAccountCommandHandler 
    : IRequestHandler<CreateUserAccountCommand, IdentityResult>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITenant _tenant;

    public CreateUserAccountCommandHandler(
        UserManager<AppUser> userManager,
        ITenant tenant
    )
    {
        _userManager = userManager;
        _tenant = tenant;
    }

    public async Task<IdentityResult> Handle(
        CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tenant.TenantId))
            throw new InvalidOperationException("Tenant context not available");

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
        };

        return string.IsNullOrEmpty(request.Password)
            ? await _userManager.CreateAsync(user)
            : await _userManager.CreateAsync(user, request.Password);
    }
}
