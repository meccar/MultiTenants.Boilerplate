using BuildingBlocks.Application.Commands.CreateUserAccount;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Tenancy.Domain.Interfaces;
using Tenancy.Domain.Models;

namespace BuildingBlocks.Application.Commands.Register;

public class CreateUserAccountCommandHandler 
    : IRequestHandler<CreateUserAccountCommand, IdentityResult>
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITenant _tenant;

    public CreateUserAccountCommandHandler(
        UserManager<IdentityUser> userManager,
        ITenant tenant
    )
    {
        _userManager = userManager;
        _tenant = tenant;
    }

    public async Task<IdentityResult> Handle(
        CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tenant.TenantName))
            throw new InvalidOperationException("Tenant context not available");

        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
        };

        return string.IsNullOrEmpty(request.Password)
            ? await _userManager.CreateAsync(user)
            : await _userManager.CreateAsync(user, request.Password);
    }
}
