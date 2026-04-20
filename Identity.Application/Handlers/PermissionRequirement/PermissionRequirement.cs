using Microsoft.AspNetCore.Authorization;

namespace Identity.Application.Handlers.PermissionRequirement;

public class PermissionRequirement : IAuthorizationRequirement
{
    public IReadOnlyList<string> RequiredPermissions { get; }

    public PermissionRequirement(params string[] permissions)
    {
        RequiredPermissions = permissions;
    }
}