namespace MultiTenants.Boilerplate.Domain.Entities;

/// <summary>
/// Pure domain user entity. No dependency on ASP.NET Core Identity.
/// </summary>
public class User
{
    public string Id { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? CreatedAt { get; set; }
}
