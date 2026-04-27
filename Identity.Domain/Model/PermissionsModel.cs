namespace Identity.Domain.Model;

public class PermissionsModel
{
    public IReadOnlyList<Guid> Ids { get; init; } = [];
    public IReadOnlyList<string> Names { get; init; } = [];
    public IReadOnlyList<string> Resources { get; init; } = [];
    public IReadOnlyList<string> Actions { get; init; } = [];
    public IReadOnlyList<string> Descriptions { get; init; } = [];
}
