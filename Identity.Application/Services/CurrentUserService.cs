using System.Security.Claims;
using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Services;

public class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    private ClaimsPrincipal? Principal => _http.HttpContext?.User;

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated is true;

    public Guid UserId => Guid.Parse(
        Principal!.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("No user in context"));

    public string Email =>
        Principal!.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public Guid? TenantId =>
        Principal?.FindFirstValue("tenant_id") is { } t ? Guid.Parse(t) : null;

    public IReadOnlyList<string> Roles =>
        Principal?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        ?? [];

    public IReadOnlyList<string> Permissions =>
        Principal?.FindAll("permission").Select(c => c.Value).ToList()
        ?? [];

    public bool HasPermission(string permission) =>
        Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
}
