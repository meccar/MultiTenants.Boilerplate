using BuildingBlocks.Domain.Seedwork.Command;
using BuildingBlocks.Shared.Dtos.UserAccount;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.CreateUserAccount;

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



