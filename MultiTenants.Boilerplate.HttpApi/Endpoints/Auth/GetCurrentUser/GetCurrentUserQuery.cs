using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Endpoints.Auth.GetCurrentUser;

public record GetCurrentUserQuery : IRequest<Result<GetCurrentUserResult>>;

public record GetCurrentUserResult(
    string? Name,
    IEnumerable<ClaimInfo> Claims);

public record ClaimInfo(string Type, string Value);

