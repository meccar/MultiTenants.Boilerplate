using BuildingBlocks.Core.Seedwork.Interface;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Tenancy.Domain.Interfaces;

namespace Identity.Application.Commands.CreateUserAccount;

public class CreateUserAccountCommandHandler 
    : TransactionalCommandHandler<CreateUserAccountCommand, IdentityResult>
{
    private readonly UserManager<UsersEntity> _userManager;
    private readonly ITenant _tenant;

    public CreateUserAccountCommandHandler(
        UserManager<UsersEntity> userManager,
        IUnitOfWork unitOfWork,
        ITenant tenant
    ) : base(unitOfWork)
    {
        _userManager = userManager;
        _tenant = tenant;
    }

    protected override async Task<IdentityResult> HandleCommandAsync(
        CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_tenant.TenantId))
            throw new InvalidOperationException("Tenant context not available");

        var user = new UsersEntity
        {
            UserName = request.Email,
            Email = request.Email,
        };

        return string.IsNullOrEmpty(request.Password)
            ? await _userManager.CreateAsync(user)
            : await _userManager.CreateAsync(user, request.Password);
    }
}
