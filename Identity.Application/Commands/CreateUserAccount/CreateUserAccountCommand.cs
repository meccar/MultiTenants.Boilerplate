using BuildingBlocks.Core.Seedwork.Command;
using BuildingBlocks.Shared.Dtos.UserAccount;
using Identity.Domain.Model;
using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Commands.CreateUserAccount;

public class CreateUserAccountCommand 
    : ICommand<IdentityResult>
{
    public CreateUserAccountCommand(
        CreateUserAccountDto userAccountDto,
        CurrentUserModel currentUser
    ) =>
    (
        Email,
        UserName,
        Password,
        CurrentUser
    ) = (
        userAccountDto.Email,
        userAccountDto.UserName,
        userAccountDto.Password,
        currentUser
    );

    public string Email { get; set; } = null!;
    public string UserName { get; set; }
    public string Password { get; set; } = null!;
    public CurrentUserModel CurrentUser { get; set; }
}
