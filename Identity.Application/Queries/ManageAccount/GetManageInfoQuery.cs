using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Queries.ManageAccount;

public record GetManageInfoQuery(string UserId) : IRequest<Result<ManageInfoDto>>;
