namespace BuildingBlocks.Shared.Models;

public record GenerateTokenRequestModel(
    string UserName,
    IList<string> Roles,
    string? TenantId = null
);
