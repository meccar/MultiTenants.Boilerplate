using BuildingBlocks.Core.Seedwork.Command;
using BuildingBlocks.Shared.Dtos.UserAccount;
using Microsoft.AspNetCore.Identity;

namespace BuildingBlocks.Application.Commands.CreateUserAccount;

public class CreateUserAccountCommand : ICommand<IdentityResult>
{
    public CreateUserAccountCommand(
        CreateUserAccountDto userAccountDto
    ) =>
    (
        Email,
        UserName,
        Password,
        TenantName
    ) = (
        userAccountDto.Email,
        userAccountDto.UserName,
        userAccountDto.Password,
        userAccountDto.TenantName
    );

    public string Email { get; set; } = null!;
    public string UserName { get; set; }
    public string Password { get; set; } = null!;
    public string TenantName { get; set; } = null!;
}



