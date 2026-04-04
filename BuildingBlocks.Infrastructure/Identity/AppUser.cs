using Microsoft.AspNetCore.Identity;

namespace BuildingBlocks.Infrastructure.Identity;

public class AppUser : IdentityUser
{
    public string TenantId { get; set; } = string.Empty;
}
