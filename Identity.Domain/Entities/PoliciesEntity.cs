using BuildingBlocks.Core.Seedwork.Aggregate;
using System.Text.Json;

namespace Identity.Domain.Entities;

public class PoliciesEntity
    : AggregateRoot
{
    public string Name { get; set; } = string.Empty;
    public string Effect { get; set; } = string.Empty;
    public JsonDocument Conditions { get; set; } = JsonDocument.Parse("{}");
}
