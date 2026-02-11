namespace MultiTenants.Boilerplate.Shared.Constants;

/// <summary>
/// Constants for validation length constraints
/// </summary>
public static class ValidationLengthConstants
{
    /// <summary>
    /// Maximum length for email addresses
    /// </summary>
    public const int EmailMaxLength = 256;

    /// <summary>
    /// Minimum length for usernames
    /// </summary>
    public const int UserNameMinLength = 3;

    /// <summary>
    /// Maximum length for usernames
    /// </summary>
    public const int UserNameMaxLength = 50;

    /// <summary>
    /// Minimum length for passwords
    /// </summary>
    public const int PasswordMinLength = 8;

    /// <summary>
    /// Maximum length for user IDs
    /// </summary>
    public const int UserIdMaxLength = 50;
}
