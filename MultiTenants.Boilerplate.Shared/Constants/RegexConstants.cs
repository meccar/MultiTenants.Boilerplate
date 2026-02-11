namespace MultiTenants.Boilerplate.Shared.Constants;

/// <summary>
/// Constants for regular expression patterns used in validation
/// </summary>
public static class RegexConstants
{
    /// <summary>
    /// Pattern for username: alphanumeric characters and underscores only
    /// </summary>
    public const string UserName = @"^[a-zA-Z0-9_]+$";

    /// <summary>
    /// Pattern to check for at least one uppercase letter
    /// </summary>
    public const string UppercaseLetter = @"[A-Z]";

    /// <summary>
    /// Pattern to check for at least one lowercase letter
    /// </summary>
    public const string LowercaseLetter = @"[a-z]";

    /// <summary>
    /// Pattern to check for at least one digit
    /// </summary>
    public const string Digit = @"[0-9]";

    /// <summary>
    /// Pattern to check for at least one special character
    /// </summary>
    public const string SpecialCharacter = @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]";
}
