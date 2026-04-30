using BuildingBlocks.Shared.Dtos.Authentication;
using BuildingBlocks.Shared.Utilities;
using MediatR;

namespace Identity.Application.Commands.Login;

/// <summary>
/// Command for OAuth (external provider) authentication.
/// Processes external login info and returns a JWT token, mirroring LocalAuthentication.
/// </summary>
public record OAuthAuthenticationCommand(
    OauthLoginDto LoginDto
) : IRequest<Result<string>>;
