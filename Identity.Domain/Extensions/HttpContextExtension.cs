using BuildingBlocks.Shared.Constants;
using BuildingBlocks.Shared.Exceptions;
using Identity.Domain.Model;
using Microsoft.AspNetCore.Http;

namespace Identity.Domain.Extensions;

public static class HttpContextExtension
{
    public static CurrentUserModel GetCurrentUser(this HttpContext context)
    {
        if (context.Items.TryGetValue(HttpContextKeys.CurrentUser, out var value)
            && value is CurrentUserModel currentUser)
            return currentUser;

        throw new UnauthorizedException();
    }

    public static CurrentUserModel? TryGetCurrentUser(this HttpContext context)
    {
        context.Items.TryGetValue(HttpContextKeys.CurrentUser, out var value);
        return value as CurrentUserModel;
    }
}