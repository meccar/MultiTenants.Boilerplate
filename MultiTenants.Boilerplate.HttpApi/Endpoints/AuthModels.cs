namespace MultiTenants.Boilerplate.Endpoints;

/// <summary>Request body for user registration.</summary>
public record RegisterRequest(string Email, string? UserName, string Password);

/// <summary>Request body for login with password.</summary>
public record LoginRequest(string EmailOrUserName, string Password);

/// <summary>Request body for forgot password.</summary>
public record ForgotPasswordRequest(string Email);

/// <summary>Request body for reset password.</summary>
public record ResetPasswordRequest(string Email, string Token, string NewPassword);

/// <summary>Request body for resend email confirmation.</summary>
public record ResendEmailConfirmationRequest(string Email);

/// <summary>Request body for change password (authenticated).</summary>
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

/// <summary>Request body for updating account (email/username).</summary>
public record UpdateManageInfoRequest(string? Email, string? UserName);

/// <summary>Request body for deleting account (password confirmation).</summary>
public record DeletePersonalDataRequest(string Password);
