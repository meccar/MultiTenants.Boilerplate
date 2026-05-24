using Identity.Domain.Model;
using MediatR;

namespace Identity.Application.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(
    string Username,
    IReadOnlyList<string>? RequiredPermissions = null
    ) 
    : IRequest<CurrentUserModel>;
