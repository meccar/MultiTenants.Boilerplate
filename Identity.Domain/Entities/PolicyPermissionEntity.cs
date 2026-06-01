using System.Text.Json;

namespace Identity.Domain.Entities;

public class PolicyPermissionEntity
{
    public Guid PolicyId { get; set; }
    public Guid PermissionId { get; set; }
    public string Effect { get; set; }
    public JsonDocument Conditions { get; set; } = JsonDocument.Parse("{}");
    public PoliciesEntity Policy { get; set; } = null!;
    public PermissionsEntity Permission { get; set; } = null!;
}
