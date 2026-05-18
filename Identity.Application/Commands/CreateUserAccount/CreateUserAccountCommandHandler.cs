using BuildingBlocks.Core.Seedwork.Interface;
using BuildingBlocks.Shared.Exceptions;
using Identity.Application.Helpers;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.CreateUserAccount;

public class CreateUserAccountCommandHandler 
    : TransactionalCommandHandler<CreateUserAccountCommand, IdentityResult>
{
    private readonly UserManager<UsersEntity> _userManager;

    public CreateUserAccountCommandHandler(
        UserManager<UsersEntity> userManager,
        IUnitOfWork unitOfWork
    ) : base(unitOfWork)
    {
        _userManager = userManager;
    }

    protected override async Task<IdentityResult> HandleCommandAsync(
        CreateUserAccountCommand request, CancellationToken cancellationToken)
    {
        var user = new UsersEntity
        {
            UserName = request.Email,
            Email = request.Email,
        };

        IdentityResult result = string.IsNullOrEmpty(request.Password) 
            ? await _userManager.CreateAsync(user)
                : await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new BadRequetException(
                IdentityErrorHelper.ToErrorDictionary(result.Errors));
        
        return result;
    }
}
