using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Shared.Dtos.UserAccount;
public class CreateUserAccountDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    [StringLength(100, MinimumLength = 12)]
    public string Password { get; set; } = null!;
}
