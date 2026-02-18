using MediatR;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands;

/// <summary>
/// Command for OAuth (external provider) authentication.
/// Processes external login info and returns a JWT token, mirroring LocalAuthentication.
/// </summary>
public record OAuthAuthenticationCommand(
    string Provider,
    string ProviderKey,
    string? Email,
    string? DisplayName,
    string TenantId
) : IRequest<Result<string>>;
