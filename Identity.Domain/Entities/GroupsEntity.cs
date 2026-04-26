using BuildingBlocks.Core.Seedwork.Aggregate;

namespace Identity.Domain.Entities;

public class GroupsEntity
    : AggregateRoot
{
    public string Name { get; set; }
    public string Description { get; set; }
}