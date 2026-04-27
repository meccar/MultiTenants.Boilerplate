using Identity.Domain.Entities;
using Identity.Domain.Model;
using MediatR;

namespace Identity.Application.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(string Token) : IRequest<CurrentUserModel>;
