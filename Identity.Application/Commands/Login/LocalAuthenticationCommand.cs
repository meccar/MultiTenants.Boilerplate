using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.Login;

public record LocalAuthenticationCommand(
  string TenantId,
  string UserName,
  string Password,
  bool IsPersistent = false
) : IRequest<Result<string>>;
