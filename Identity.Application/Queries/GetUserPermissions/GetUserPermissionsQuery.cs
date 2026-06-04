using Identity.Domain.Model;
using MediatR;

namespace Identity.Application.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(
    string Username,
    IEnumerable<string>? RequiredPermissions = null
    ) 
    : IRequest<CurrentUserModel>;
