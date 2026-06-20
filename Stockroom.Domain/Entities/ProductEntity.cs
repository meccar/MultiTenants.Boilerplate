namespace Stockroom.Domain.Entities;

public class ProductEntity
{
    public Guid StoreId { get; set; }
    public Guid CategoryId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public string Unit { get; private set; } = string.Empty;
    public string? Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
}