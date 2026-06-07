namespace BuildingBlocks.Shared.Models;

public record GenerateTokenRequestModel(
    string UserName,
    IList<string> Roles,
    string SecurityStamp,
    string? TenantId = null
);
