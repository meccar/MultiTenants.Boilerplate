using System.Security.Claims;
using BuildingBlocks.Shared.Constants;
using BuildingBlocks.Shared.Exceptions;
using Identity.Application.Queries.GetUserPermissions;
using Identity.Domain.Model;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;

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
            
            var username = httpContext?.User.FindFirstValue("unique_name")
                           ?? httpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                           ?? httpContext?.User.FindFirstValue(ClaimTypes.Name)
                           ?? httpContext?.User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(username))
            {
                context.Fail();
                return;
            }

            if (httpContext?.Items.TryGetValue(
                    HttpContextKeys.CurrentUser, out var cached) == true
                && cached is CurrentUserModel cachedUser)
            {
                var isAllowed = requirement.RequiredPermissions
                    .All(p => cachedUser.Permissions
                        .Any(x => x.Name.Equals(p, StringComparison.OrdinalIgnoreCase)));

                if (isAllowed) context.Succeed(requirement);
                else context.Fail();
                return;
            }

            var result = await _sender.Send(new GetUserPermissionsQuery(
                Username: username,
                RequiredPermissions: requirement.RequiredPermissions.ToList()
            ));

            if (httpContext != null)
                httpContext.Items[HttpContextKeys.CurrentUser] = result;

            if (result.IsAllowed) context.Succeed(requirement);
            else context.Fail();
        }
        catch (Exception ex)
            when (ex is UnauthorizedAccessException or UnauthorizedException)
        {
            context.Fail();
        }
    }
}