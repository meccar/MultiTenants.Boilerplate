using BuildingBlocks.Shared.Dtos.Authentication;
using MediatR;

namespace Identity.Application.Commands.Login;

public record LocalAuthenticationCommand(
  LocalLoginDto LoginDto
) : IRequest<TokenDto>;
