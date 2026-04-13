using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.Login;

public record LocalAuthenticationCommand(
  string UserName,
  string Password,
  bool IsPersistent = false
) : IRequest<Result<string>>;
