using Identity.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Application.Handlers.PermissionRequirement;

public class PermissionRequirementHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly ICurrentUser _currentUser;

    public PermissionRequirementHandler(
        ICurrentUser currentUser)
            => _currentUser = currentUser;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (!_currentUser.IsAuthenticated)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var satisfied = requirement.RequiredPermissions
            .All(_currentUser.HasPermission);

        if (satisfied)
            context.Succeed(requirement);
        else
            context.Fail(new AuthorizationFailureReason(this,
                $"Missing: {string.Join(", ",
                    requirement.RequiredPermissions
                        .Where(p => !_currentUser.HasPermission(p)))}"));

        return Task.CompletedTask;
    }
}