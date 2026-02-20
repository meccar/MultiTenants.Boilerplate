using Carter;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiTenants.Boilerplate.Application.Abstractions;
using MultiTenants.Boilerplate.Application.Services;
using MultiTenants.Boilerplate.Configurations;
using MultiTenants.Boilerplate.Infrastructure.Identity;
using MultiTenants.Boilerplate.Shared.Constants;
using MultiTenants.Boilerplate.Shared.Responses;
using Microsoft.Extensions.Options;
using System.Net;

namespace MultiTenants.Boilerplate.Endpoints;

public class AuthEndpoints : ICarterModule
{
    [Obsolete("Obsolete")]
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var basePath = app.GetApiBasePath();
        var group = app.MapGroup($"{basePath}/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        // ---------- Registration ----------
        group.MapPost("/register", Register)
            .WithName("Register")
            .WithSummary("Register a new user")
            .AllowAnonymous();

        // ---------- Login (password) ----------
        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Login with email/userName and password")
            .AllowAnonymous();

        // ---------- Logout ----------
        group.MapPost("/logout", Logout)
            .WithName("Logout")
            .WithSummary("Logout current user")
            .RequireAuthorization();

        // ---------- Forgot password ----------
        group.MapPost("/forgot-password", ForgotPassword)
            .WithName("ForgotPassword")
            .WithSummary("Request a password reset email")
            .AllowAnonymous();

        // ---------- Reset password ----------
        group.MapPost("/reset-password", ResetPassword)
            .WithName("ResetPassword")
            .WithSummary("Reset password using token from email")
            .AllowAnonymous();

        // ---------- Confirm email ----------
        group.MapGet("/confirm-email", ConfirmEmail)
            .WithName("ConfirmEmail")
            .WithSummary("Confirm email using token from email")
            .AllowAnonymous();

        // ---------- Resend email confirmation ----------
        group.MapPost("/resend-email-confirmation", ResendEmailConfirmation)
            .WithName("ResendEmailConfirmation")
            .WithSummary("Resend email confirmation link")
            .AllowAnonymous();

        // ---------- Change password (authenticated) ----------
        group.MapPost("/change-password", ChangePassword)
            .WithName("ChangePassword")
            .WithSummary("Change password for current user")
            .RequireAuthorization();

        // ---------- External login (Google) ----------
        group.MapGet("/login/google", LoginWithGoogle)
            .WithName("LoginWithGoogle")
            .WithSummary("Initiate Google OAuth login")
            .AllowAnonymous();

        group.MapGet("/login/google/callback", GoogleCallback)
            .WithName("GoogleCallback")
            .WithSummary("Google OAuth callback")
            .AllowAnonymous();

        // ---------- Current user ----------
        group.MapGet("/me", GetCurrentUser)
            .WithName("GetCurrentUser")
            .WithSummary("Get current authenticated user")
            .RequireAuthorization();

        // ---------- Manage: two-factor (stub - requires IUserTwoFactorStore) ----------
        group.MapGet("/manage/2fa", GetTwoFactorStatus)
            .WithName("GetTwoFactorStatus")
            .WithSummary("Get two-factor authentication status")
            .RequireAuthorization();

        // ---------- Manage account ----------
        group.MapGet("/manage/info", GetManageInfo)
            .WithName("GetManageInfo")
            .WithSummary("Get current user account info for management")
            .RequireAuthorization();

        group.MapPost("/manage/info", UpdateManageInfo)
            .WithName("UpdateManageInfo")
            .WithSummary("Update email and/or username")
            .RequireAuthorization();

        group.MapPost("/manage/delete", DeletePersonalData)
            .WithName("DeletePersonalData")
            .WithSummary("Delete account and personal data")
            .RequireAuthorization();
    }

    private static async Task<IResult> Register(
        [FromBody] RegisterRequest request,
        [FromServices] UserManager<AppUser> userManager,
        [FromServices] ITenantProvider tenantProvider,
        [FromServices] IEmailSender emailSender,
        [FromServices] IConfiguration configuration,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Results.Json(ApiResponse.BadRequest("Email and password are required."), statusCode: (int)HttpStatusCode.BadRequest);

        var userName = request.UserName ?? request.Email;
        var user = new AppUser
        {
            TenantId = tenantProvider.GetCurrentTenantId() ?? string.Empty,
            UserName = userName,
            Email = request.Email,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return Results.Json(ApiResponse<object>.ValidationError(errors), statusCode: (int)HttpStatusCode.BadRequest);
        }

        // Send confirmation email
        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var baseUrl = GetBaseUrl(configuration, httpContextAccessor);
        var callbackUrl = $"{baseUrl}/confirm-email?userId={Uri.EscapeDataString(user.Id)}&code={Uri.EscapeDataString(code)}";
        await emailSender.SendEmailAsync(
            request.Email,
            "Confirm your email",
            $"Please confirm your account by visiting: {callbackUrl}",
            cancellationToken);

        return Results.Json(ApiResponse<string>.SuccessResponse(user.Id, "Registration successful. Please check your email to confirm your account."));
    }

    private static async Task<IResult> Login(
        [FromBody] LoginRequest request,
        [FromServices] SignInManager<AppUser> signInManager,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EmailOrUserName) || string.IsNullOrWhiteSpace(request.Password))
            return Results.Json(ApiResponse.Unauthorized("Email/userName and password are required."), statusCode: (int)HttpStatusCode.Unauthorized);

        var isEmail = request.EmailOrUserName.Contains('@');
        var user = isEmail
            ? await signInManager.UserManager.FindByEmailAsync(request.EmailOrUserName)
            : await signInManager.UserManager.FindByNameAsync(request.EmailOrUserName);

        if (user == null)
            return Results.Json(ApiResponse.Unauthorized("Invalid login attempt."), statusCode: (int)HttpStatusCode.Unauthorized);

        var result = await signInManager.PasswordSignInAsync(user.UserName!, request.Password, isPersistent: false, lockoutOnFailure: true);
        if (result.IsLockedOut)
            return Results.Json(ApiResponse.Forbidden("Account locked out."), statusCode: (int)HttpStatusCode.Forbidden);
        if (!result.Succeeded)
            return Results.Json(ApiResponse.Unauthorized("Invalid login attempt."), statusCode: (int)HttpStatusCode.Unauthorized);

        return Results.Json(ApiResponse.SuccessResponse("Logged in successfully."));
    }

    private static async Task<IResult> Logout(HttpContext context)
    {
        await context.SignOutAsync(AuthConstants.DefaultScheme);
        return Results.Json(ApiResponse.SuccessResponse("Logged out successfully."));
    }

    private static async Task<IResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        [FromServices] UserManager<AppUser> userManager,
        [FromServices] IEmailSender emailSender,
        [FromServices] IConfiguration configuration,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Results.Json(ApiResponse.BadRequest("Email is required."), statusCode: (int)HttpStatusCode.BadRequest);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Results.Ok(ApiResponse.SuccessResponse("If an account exists for this email, a reset link has been sent."));

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var baseUrl = GetBaseUrl(configuration, httpContextAccessor);
        var callbackUrl = $"{baseUrl}/reset-password?email={Uri.EscapeDataString(request.Email)}&token={Uri.EscapeDataString(token)}";
        await emailSender.SendEmailAsync(
            request.Email,
            "Reset your password",
            $"Reset your password by visiting: {callbackUrl}",
            cancellationToken);

        return Results.Json(ApiResponse.SuccessResponse("If an account exists for this email, a reset link has been sent."));
    }

    private static async Task<IResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        [FromServices] UserManager<AppUser> userManager,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            return Results.Json(ApiResponse.BadRequest("Email, token, and new password are required."), statusCode: (int)HttpStatusCode.BadRequest);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Results.Json(ApiResponse.BadRequest("Invalid request."), statusCode: (int)HttpStatusCode.BadRequest);

        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return Results.Json(ApiResponse<object>.ValidationError(errors), statusCode: (int)HttpStatusCode.BadRequest);
        }

        return Results.Json(ApiResponse.SuccessResponse("Password has been reset."));
    }

    private static async Task<IResult> ConfirmEmail(
        [FromQuery] string userId,
        [FromQuery] string code,
        [FromServices] UserManager<AppUser> userManager,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            return Results.Json(ApiResponse.BadRequest("UserId and code are required."), statusCode: (int)HttpStatusCode.BadRequest);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Results.Json(ApiResponse.NotFound("User not found."), statusCode: (int)HttpStatusCode.NotFound);

        var result = await userManager.ConfirmEmailAsync(user, code);
        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return Results.Json(ApiResponse<object>.ValidationError(errors), statusCode: (int)HttpStatusCode.BadRequest);
        }

        return Results.Json(ApiResponse.SuccessResponse("Email confirmed."));
    }

    private static async Task<IResult> ResendEmailConfirmation(
        [FromBody] ResendEmailConfirmationRequest request,
        [FromServices] UserManager<AppUser> userManager,
        [FromServices] IEmailSender emailSender,
        [FromServices] IConfiguration configuration,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Results.Json(ApiResponse.BadRequest("Email is required."), statusCode: (int)HttpStatusCode.BadRequest);

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Results.Json(ApiResponse.SuccessResponse("If an account exists for this email, a confirmation link has been sent."));

        if (user.EmailConfirmed)
            return Results.Json(ApiResponse.BadRequest("Email is already confirmed."), statusCode: (int)HttpStatusCode.BadRequest);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var baseUrl = GetBaseUrl(configuration, httpContextAccessor);
        var callbackUrl = $"{baseUrl}/confirm-email?userId={Uri.EscapeDataString(user.Id)}&code={Uri.EscapeDataString(code)}";
        await emailSender.SendEmailAsync(
            request.Email,
            "Confirm your email",
            $"Please confirm your account by visiting: {callbackUrl}",
            cancellationToken);

        return Results.Json(ApiResponse.SuccessResponse("If an account exists for this email, a confirmation link has been sent."));
    }

    private static async Task<IResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        HttpContext context,
        [FromServices] UserManager<AppUser> userManager,
        [FromServices] SignInManager<AppUser> signInManager,
        CancellationToken cancellationToken)
    {
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Results.Json(ApiResponse.Unauthorized(), statusCode: (int)HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Results.Json(ApiResponse.NotFound("User not found."), statusCode: (int)HttpStatusCode.NotFound);

        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return Results.Json(ApiResponse<object>.ValidationError(errors), statusCode: (int)HttpStatusCode.BadRequest);
        }

        await signInManager.RefreshSignInAsync(user);
        return Results.Json(ApiResponse.SuccessResponse("Password changed successfully."));
    }

    private static IResult LoginWithGoogle(
        HttpContext context,
        [FromServices] IOptions<ApiOptions> apiOptions,
        [FromQuery] string? returnUrl = null)
    {
        var basePath = apiOptions.Value.BasePath;
        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl ?? $"{basePath}/auth/login/google/callback"
        };
        return Results.Challenge(properties, new[] { AuthConstants.GoogleScheme });
    }

    private static async Task<IResult> GoogleCallback(HttpContext context)
    {
        var result = await context.AuthenticateAsync(AuthConstants.GoogleScheme);
        if (!result.Succeeded)
            return Results.Unauthorized();

        await context.SignInAsync(AuthConstants.DefaultScheme, result.Principal!);
        return Results.Json(ApiResponse.SuccessResponse("Authentication successful."));
    }

    private static IResult GetCurrentUser(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
            return Results.Unauthorized();

        var userInfo = new
        {
            context.User.Identity.Name,
            UserId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? context.User.FindFirst("sub")?.Value,
            Claims = context.User.Claims.Select(c => new { c.Type, c.Value })
        };
        return Results.Ok(userInfo);
    }

    private static IResult GetTwoFactorStatus(HttpContext context, [FromServices] UserManager<AppUser> userManager)
    {
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Results.Json(ApiResponse<object>.Unauthorized(), statusCode: (int)HttpStatusCode.Unauthorized);

        // Two-factor requires IUserTwoFactorStore; MongoUserStore does not implement it yet
        var twoFactorEnabled = false;
        return Results.Json(ApiResponse<object>.SuccessResponse(new { twoFactorEnabled }));
    }

    private static async Task<IResult> GetManageInfo(
        HttpContext context,
        [FromServices] UserManager<AppUser> userManager)
    {
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Results.Json(ApiResponse<object>.Unauthorized(), statusCode: (int)HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Results.Json(ApiResponse<object>.NotFound(), statusCode: (int)HttpStatusCode.NotFound);

        var info = new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.EmailConfirmed,
            user.PhoneNumber,
            user.TwoFactorEnabled
        };
        return Results.Json(ApiResponse<object>.SuccessResponse(info));
    }

    private static async Task<IResult> UpdateManageInfo(
        [FromBody] UpdateManageInfoRequest request,
        HttpContext context,
        [FromServices] UserManager<AppUser> userManager,
        [FromServices] SignInManager<AppUser> signInManager,
        CancellationToken cancellationToken)
    {
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Results.Json(ApiResponse.Unauthorized(), statusCode: (int)HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Results.Json(ApiResponse.NotFound(), statusCode: (int)HttpStatusCode.NotFound);

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            await userManager.SetEmailAsync(user, request.Email);
            user.EmailConfirmed = false;
        }
        if (!string.IsNullOrWhiteSpace(request.UserName) && request.UserName != user.UserName)
            await userManager.SetUserNameAsync(user, request.UserName);

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return Results.Json(ApiResponse<object>.ValidationError(errors), statusCode: (int)HttpStatusCode.BadRequest);
        }

        await signInManager.RefreshSignInAsync(user);
        return Results.Json(ApiResponse.SuccessResponse("Account updated successfully."));
    }

    private static async Task<IResult> DeletePersonalData(
        [FromBody] DeletePersonalDataRequest request,
        HttpContext context,
        [FromServices] UserManager<AppUser> userManager,
        [FromServices] SignInManager<AppUser> signInManager,
        CancellationToken cancellationToken)
    {
        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? context.User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
            return Results.Json(ApiResponse.Unauthorized(), statusCode: (int)HttpStatusCode.Unauthorized);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return Results.Json(ApiResponse.NotFound(), statusCode: (int)HttpStatusCode.NotFound);

        var isValidPassword = await userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
            return Results.Json(ApiResponse.BadRequest("Password is required to delete your account."), statusCode: (int)HttpStatusCode.BadRequest);

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(e => e.Code).ToDictionary(g => g.Key, g => g.Select(e => e.Description).ToArray());
            return Results.Json(ApiResponse<object>.ValidationError(errors), statusCode: (int)HttpStatusCode.BadRequest);
        }

        await context.SignOutAsync(AuthConstants.DefaultScheme);
        return Results.Json(ApiResponse.SuccessResponse("Account deleted successfully."));
    }

    private static string GetBaseUrl(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        var baseUrl = configuration["App:BaseUrl"];
        if (!string.IsNullOrWhiteSpace(baseUrl))
            return baseUrl.TrimEnd('/');

        var request = httpContextAccessor.HttpContext?.Request;
        if (request != null)
        {
            var scheme = request.Scheme;
            var host = request.Host.Value;
            return $"{scheme}://{host}";
        }

        return "https://localhost";
    }
}
