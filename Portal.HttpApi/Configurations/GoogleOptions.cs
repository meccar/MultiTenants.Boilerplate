using BuildingBlocks.Shared.Constants;

namespace Host.Configurations;

public sealed class GoogleOptions
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string AuthorizationEndpoint { get; init; } = AuthConstants.GoogleAuthorizationUrl;
    public string TokenEndpoint { get; init; } = AuthConstants.GoogleTokenUrl;
}
