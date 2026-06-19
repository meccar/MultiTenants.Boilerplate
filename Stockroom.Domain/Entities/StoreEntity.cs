using BuildingBlocks.Core.Seedwork.Aggregate;
using BuildingBlocks.Core.Seedwork.ValueObject;
using BuildingBlocks.Shared.Enums;

namespace Stockroom.Domain.Entities;

public class StoreEntity : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public EStoreType Type { get; set; }
    public string? Description { get; private set; }
    public Address Address { get; private set; } = default!;
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; } = true;
}