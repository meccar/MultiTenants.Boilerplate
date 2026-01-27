namespace MultiTenants.Boilerplate.Shared.Constants;

public static class AuthConstants
{
    public const string GoogleScheme = "Google";
    public const string CookieScheme = "Cookies";
    public const string DefaultScheme = "Identity.Application"; // Used by Identity
    
    // Google OAuth URLs - can be overridden via configuration if needed
    public const string GoogleAuthorizationUrl = "https://accounts.google.com/o/oauth2/v2/auth";
    public const string GoogleTokenUrl = "https://oauth2.googleapis.com/token";
}

