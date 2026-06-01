using System.Text.Json;
using BuildingBlocks.Core.Seedwork.Aggregate;

namespace Identity.Domain.Entities;

public class PermissionsEntity
    : AggregateRoot
{
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public ICollection<PolicyPermissionEntity> PolicyPermissions { get; set; } = [];
}