using Microsoft.AspNetCore.Identity;

namespace BuildingBlocks.Infrastructure.Identity;

public class AppRole : IdentityRole
{
    public string TenantId { get; set; } = string.Empty;
}
