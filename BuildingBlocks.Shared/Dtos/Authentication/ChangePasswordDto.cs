using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Shared.Dtos.Authentication;

public class ChangePasswordDto
{
    public string? UserName { get; set; }
    
    [Required]
    public string? CurrentPassword { get; set; }
    
    [Required]
    public string NewPassword { get; set; }
}