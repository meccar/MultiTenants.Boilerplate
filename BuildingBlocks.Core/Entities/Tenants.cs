using BuildingBlocks.Domain.Seedwork.Aggregate;

namespace BuildingBlocks.Core.Entities;
public class Tenants : AggregateRoot
{
    public string Name { get; set; } = null!;
    public string Domain { get; set; } = null!;
    public string Status { get; set; } = null!;
}
