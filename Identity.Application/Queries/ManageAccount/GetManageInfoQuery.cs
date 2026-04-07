using MediatR;
using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Queries.ManageAccount;

public record GetManageInfoQuery(string UserId) : IRequest<Result<ManageInfoDto>>;
