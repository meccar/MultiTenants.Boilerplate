using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Queries.GetTwoFactorStatus;

public record GetTwoFactorStatusQuery(string UserId) : IRequest<Result<bool>>;
