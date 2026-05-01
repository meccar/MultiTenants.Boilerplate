using System.ComponentModel.DataAnnotations;

namespace BuildingBlocks.Shared.Dtos.Authentication;

public class OauthLoginDto
{
    [Required]
    public string Provider { get; set; }
    
    [Required]
    public string ProviderKey { get; set; }
}