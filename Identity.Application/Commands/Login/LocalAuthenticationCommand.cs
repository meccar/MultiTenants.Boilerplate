using BuildingBlocks.Shared.Dtos.Authentication;
using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Commands.Login;

public record LocalAuthenticationCommand(
  LocalLoginDto LoginDto
) : IRequest<Result<string>>;
