namespace Identity.Domain.Model;

public class PoliciesModel
{
    public IReadOnlyList<Guid> Ids { get; init; } = [];
    public IReadOnlyList<string> Names { get; init; } = [];
    public IReadOnlyList<string> Effects { get; init; } = [];
    public IReadOnlyList<string> Conditions { get; init; } = [];
}
