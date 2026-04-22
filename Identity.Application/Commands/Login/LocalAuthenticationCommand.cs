using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Commands.Login;

public record LocalAuthenticationCommand(
  string UserName,
  string Password,
  bool IsPersistent = false
) : IRequest<Result<string>>;
