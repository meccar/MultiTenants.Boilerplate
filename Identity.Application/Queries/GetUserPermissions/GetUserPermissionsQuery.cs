using Identity.Domain.Entities;
using Identity.Domain.Model;
using MediatR;

namespace Identity.Application.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(
    IReadOnlyList<string>? RequiredPermissions = null
    ) 
    : IRequest<CurrentUserModel>;
