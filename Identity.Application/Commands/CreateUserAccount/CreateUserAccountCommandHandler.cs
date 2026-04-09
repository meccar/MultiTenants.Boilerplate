using BuildingBlocks.Application.Commands.CreateUserAccount;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BuildingBlocks.Application.Commands.Register;

public class CreateUserAccountCommandHandler 
    : IRequestHandler<CreateUserAccountCommand, IdentityResult>
{
    private readonly UserManager<IdentityUser> _userManager;

    public CreateUserAccountCommandHandler(
        UserManager<IdentityUser> userManager
    ){
        _userManager = userManager;
    }

    public async Task<IdentityResult> Handle(
        CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        //var tenantId = _tenantProvider.GetCurrentTenantId();
        //if (string.IsNullOrEmpty(tenantId))
        //    return Result<string>.Failure("Tenant context not found");

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
