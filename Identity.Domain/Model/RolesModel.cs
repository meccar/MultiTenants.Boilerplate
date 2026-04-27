namespace Identity.Domain.Model;

public class RolesModel
{
    public IReadOnlyList<Guid> Ids { get; init; } = [];
    public IReadOnlyList<string> Names { get; init; } = [];
}
