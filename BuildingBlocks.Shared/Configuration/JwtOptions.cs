namespace BuildingBlocks.Shared.Configuration;

public sealed class JwtOptions
{
    public string Secret { get; init; } = default!;
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public int AccessExpiry { get; init; } = default!;
    public int RefreshExpiry { get; init; } = default!;
}