using MultiTenants.Boilerplate.Domain.Seedwork.Command;
using MultiTenants.Boilerplate.Shared.Dtos.UserAccount;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands.CreateUserAccount;

public class CreateUserAccountCommand : ICommand<Result<string>>
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



