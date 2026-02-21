namespace MultiTenants.Boilerplate.Application.Abstractions;

/// <summary>
/// Represents the current tenant in the request context (id, name, identifier).
/// Mapped from the multi-tenant resolution layer so Application does not reference Finbuckle.
/// </summary>
public sealed record CurrentTenant
{
    /// <summary>Unique identifier for the tenant (stable, never change).</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Display name of the tenant.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Value used to resolve the tenant (e.g. host, path, claim).</summary>
    public string Identifier { get; init; } = string.Empty;
}
