using BuildingBlocks.Core.Seedwork.Aggregate;
using BuildingBlocks.Core.Seedwork.ValueObject;
using BuildingBlocks.Shared.Enums;

namespace Stockroom.Domain.Entities;

public class StoreEntity : AggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public EStoreType Type { get; set; }
    public string? Description { get; set; }
    public Address Address { get; set; } = default!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
}
