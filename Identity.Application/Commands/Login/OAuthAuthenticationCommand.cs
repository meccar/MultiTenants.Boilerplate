using MediatR;
using BuildingBlocks.Shared.Utilities;

namespace BuildingBlocks.Application.Commands.Login;

/// <summary>
/// Command for OAuth (external provider) authentication.
/// Processes external login info and returns a JWT token, mirroring LocalAuthentication.
/// </summary>
public record OAuthAuthenticationCommand(
    string Provider,
    string ProviderKey,
    string? Email,
    string? DisplayName
) : IRequest<Result<string>>;
