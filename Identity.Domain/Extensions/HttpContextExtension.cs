using BuildingBlocks.Shared.Constants;
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

        throw new UnauthorizedAccessException();
    }

    public static CurrentUserModel? TryGetCurrentUser(this HttpContext context)
    {
        context.Items.TryGetValue(HttpContextKeys.CurrentUser, out var value);
        return value as CurrentUserModel;
    }
}