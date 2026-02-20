namespace MultiTenants.Boilerplate.Domain.Entities;

/// <summary>
/// Pure domain role entity. No dependency on ASP.NET Core Identity.
/// </summary>
public class Role
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
