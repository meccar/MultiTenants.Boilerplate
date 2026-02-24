using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Queries.GetTwoFactorStatus;

public record GetTwoFactorStatusQuery(string UserId) : IRequest<Result<bool>>;
