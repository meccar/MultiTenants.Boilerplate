using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Shared.Dtos.Authentication;

public class LocalLoginDto
{
    [Required]
    public string UserName { get; set; }
    
    [Required]
    public string Password { get; set; }

    public bool IsPersistent { get; set; } = false;
}