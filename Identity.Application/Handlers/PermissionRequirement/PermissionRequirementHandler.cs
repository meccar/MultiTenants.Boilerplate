using BuildingBlocks.Shared.Constants;
using Identity.Application.Queries.GetUserPermissions;
using Identity.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Identity.Application.Handlers.PermissionRequirement;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly ISender _sender;

    public PermissionAuthorizationHandler(
        ISender sender)
            => _sender = sender;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        try
        {
            var httpContext = context.Resource as HttpContext;

            CurrentUserModel result;
            if (httpContext?.Items.TryGetValue(HttpContextKeys.CurrentUser, out var cached) == true
                && cached is CurrentUserModel cachedUser)
            {
                var isAllowed = requirement.RequiredPermissions
                    .All(p => cachedUser.Permissions.Names
                        .Any(n => n.Equals(p, StringComparison.OrdinalIgnoreCase)));

                if (isAllowed)
                    context.Succeed(requirement);
                else
                    context.Fail();

                return;
            }
            else
            {
                result = await _sender.Send(new GetUserPermissionsQuery
                {
                    RequiredPermissions = requirement.RequiredPermissions.ToList()
                });

                if (httpContext != null)
                    httpContext.Items[HttpContextKeys.CurrentUser] = result;
            }

            if (result.IsAllowed)
                context.Succeed(requirement);
            else
                context.Fail();
        }
        catch (UnauthorizedAccessException)
        {
            context.Fail();
        }
    }
}