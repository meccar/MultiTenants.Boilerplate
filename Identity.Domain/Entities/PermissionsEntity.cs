using BuildingBlocks.Core.Seedwork.Aggregate;

namespace Identity.Domain.Entities;

public class PermissionsEntity
    : AggregateRoot
{
    public string Name { get; set; }
    public string Resource { get; set; }
    public string Action { get; set; }
    public string Description { get; set; }
}