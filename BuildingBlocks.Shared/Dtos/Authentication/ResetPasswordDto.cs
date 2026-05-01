using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Shared.Dtos.Authentication;

public class ResetPasswordDto
{
    public string UserName { get; set; }
    
    [Required]
    public string NewPassword { get; set; }
}