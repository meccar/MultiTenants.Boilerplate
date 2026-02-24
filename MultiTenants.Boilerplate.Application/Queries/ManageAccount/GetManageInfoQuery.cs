using MediatR;
using MultiTenants.Boilerplate.Application.DTOs;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Queries.ManageAccount;

public record GetManageInfoQuery(string UserId) : IRequest<Result<ManageInfoDto>>;
