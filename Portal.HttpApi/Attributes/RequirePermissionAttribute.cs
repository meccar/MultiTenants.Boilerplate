using Identity.Application.Providers;
using Microsoft.AspNetCore.Authorization;

namespace Host.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permission)
        : base($"{PermissionPolicyProvider.PolicyPrefix}{permission}") { }
}