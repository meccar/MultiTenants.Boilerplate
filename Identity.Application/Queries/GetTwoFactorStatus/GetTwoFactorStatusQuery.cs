using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Queries.GetTwoFactorStatus;

public record GetTwoFactorStatusQuery(string UserId) : IRequest<Result<bool>>;
