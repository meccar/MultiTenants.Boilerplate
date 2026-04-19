namespace Identity.Domain.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
    Guid? TenantId { get; }
    IReadOnlyList<string> Roles { get; }
    IReadOnlyList<string> Permissions { get; }
    bool IsAuthenticated { get; }
    bool HasPermission(string permission);
}