using BuildingBlocks.Core.Seedwork.Aggregate;

namespace Stockroom.Domain.Entities;

public class CategoryEntity : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public Guid StoreId { get; set; }
    public int SortOrder { get; set; } = 0;
}