using Identity.Domain.Entities;
using MediatR;

namespace Identity.Application.Queries.GetUserPermissions;

public record GetUserPermissionsQuery(string Token) : IRequest<AppUser>;
