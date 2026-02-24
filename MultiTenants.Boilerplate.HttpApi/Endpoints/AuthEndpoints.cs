using Carter;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MultiTenants.Boilerplate.Application.Commands.ChangePassword;
using MultiTenants.Boilerplate.Application.Commands.ConfirmEmail;
using MultiTenants.Boilerplate.Application.Commands.ForgotPassword;
using MultiTenants.Boilerplate.Application.Commands.Login;
using MultiTenants.Boilerplate.Application.Commands.Logout;
using MultiTenants.Boilerplate.Application.Commands.ManageAccount;
using MultiTenants.Boilerplate.Application.Commands.Register;
using MultiTenants.Boilerplate.Application.Commands.ResendEmailConfirmation;
using MultiTenants.Boilerplate.Application.Commands.ResetPassword;
using MultiTenants.Boilerplate.Application.Queries.GetCurrentUser;
using MultiTenants.Boilerplate.Application.Queries.GetTwoFactorStatus;
using MultiTenants.Boilerplate.Application.Queries.ManageAccount;
using MultiTenants.Boilerplate.Configurations;
using MultiTenants.Boilerplate.Shared.Constants;
using MultiTenants.Boilerplate.Shared.Responses;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;

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
        group.MapPost("/register", async (
                [FromBody] CreateUserCommand command,
                [FromServices] ISender sender) =>
            {
                var result = await sender.Send(command);
                return result.IsSuccess
                    ? Results.Json(ApiResponse<string>.SuccessResponse(result.Value,
                        "Registration successful. Please check your email to confirm your account."))
                    : Results.Json(ApiResponse<string>.BadRequest(result.Error),
                        statusCode: (int)HttpStatusCode.BadRequest);
            })
            .WithName("Register")
            .WithSummary("Register a new user")
            .AllowAnonymous();

        // ---------- Login (password) ----------
        group.MapPost("/login", async (
                [FromBody] LocalAuthenticationCommand command,
                [FromServices] ISender sender) =>
            {
                var result = await sender.Send(command);
                return result.IsSuccess
                    ? Results.Json(ApiResponse<string>.SuccessResponse(result.Value, "Logged in successfully."))
                    : Results.Json(ApiResponse.Unauthorized(result.Error),
                        statusCode: (int)HttpStatusCode.Unauthorized);
            })
            .WithName("Login")
            .WithSummary("Login with email/userName and password")
            .AllowAnonymous();

        // ---------- Logout ----------
        group.MapPost("/logout", async ([FromServices] ISender sender) =>
            {
                await sender.Send(new LogoutCommand());
                return Results.Json(ApiResponse.SuccessResponse("Logged out successfully."));
            })
            .WithName("Logout")
            .WithSummary("Logout current user")
            .RequireAuthorization();

        // ---------- Forgot password ----------
        group.MapPost("/forgot-password", async (
                [FromBody] ForgotPasswordCommand command,
                [FromServices] ISender sender) =>
            {
                await sender.Send(command);
                // Always return success to prevent email enumeration
                return Results.Json(ApiResponse.SuccessResponse(
                    "If an account exists for this email, a reset link has been sent."));
            })
            .WithName("ForgotPassword")
            .WithSummary("Request a password reset email")
            .AllowAnonymous();

        // ---------- Reset password ----------
        group.MapPost("/reset-password", async (
                [FromBody] ResetPasswordCommand command,
                [FromServices] ISender sender) =>
            {
                var result = await sender.Send(command);
                return result.IsSuccess
                    ? Results.Json(ApiResponse.SuccessResponse("Password has been reset."))
                    : Results.Json(ApiResponse.BadRequest(result.Error),
                        statusCode: (int)HttpStatusCode.BadRequest);
            })
            .WithName("ResetPassword")
            .WithSummary("Reset password using token from email")
            .AllowAnonymous();

        // ---------- Confirm email ----------
        group.MapGet("/confirm-email", async (
                [FromQuery] string userId,
                [FromQuery] string code,
                [FromServices] ISender sender) =>
            {
                var result = await sender.Send(new ConfirmEmailCommand(userId, code));
                return result.IsSuccess
                    ? Results.Json(ApiResponse.SuccessResponse("Email confirmed."))
                    : Results.Json(ApiResponse.BadRequest(result.Error),
                        statusCode: (int)HttpStatusCode.BadRequest);
            })
            .WithName("ConfirmEmail")
            .WithSummary("Confirm email using token from email")
            .AllowAnonymous();

        // ---------- Resend email confirmation ----------
        group.MapPost("/resend-email-confirmation", async (
                [FromBody] ResendEmailConfirmationCommand command,
                [FromServices] ISender sender) =>
            {
                var result = await sender.Send(command);
                return result.IsSuccess
                    ? Results.Json(ApiResponse.SuccessResponse(
                        "If an account exists for this email, a confirmation link has been sent."))
                    : Results.Json(ApiResponse.BadRequest(result.Error),
                        statusCode: (int)HttpStatusCode.BadRequest);
            })
            .WithName("ResendEmailConfirmation")
            .WithSummary("Resend email confirmation link")
            .AllowAnonymous();

        // ---------- Change password ----------
        group.MapPost("/change-password", async (
                [FromBody] ChangePasswordRequest request,
                HttpContext context,
                [FromServices] ISender sender) =>
            {
                var userId = GetUserId(context);
                if (userId is null)
                    return Results.Json(ApiResponse.Unauthorized(),
                        statusCode: (int)HttpStatusCode.Unauthorized);

                var result = await sender.Send(
                    new ChangePasswordCommand(userId, request.CurrentPassword, request.NewPassword));
                return result.IsSuccess
                    ? Results.Json(ApiResponse.SuccessResponse("Password changed successfully."))
                    : Results.Json(ApiResponse.BadRequest(result.Error),
                        statusCode: (int)HttpStatusCode.BadRequest);
            })
            .WithName("ChangePassword")
            .WithSummary("Change password for current user")
            .RequireAuthorization();

        // ---------- External login (Google) ----------
        group.MapGet("/login/google", (
                HttpContext context,
                [FromServices] IOptions<ApiOptions> apiOptions,
                [FromQuery] string? returnUrl = null) =>
            {
                var basePath2 = apiOptions.Value.BasePath;
                var properties = new AuthenticationProperties
                {
                    RedirectUri = returnUrl ?? $"{basePath2}/auth/login/google/callback"
                };
                return Results.Challenge(properties, [AuthConstants.GoogleScheme]);
            })
            .WithName("LoginWithGoogle")
            .WithSummary("Initiate Google OAuth login")
            .AllowAnonymous();

        group.MapGet("/login/google/callback", async (
                HttpContext context,
                [FromServices] ISender sender) =>
            {
                var authResult = await context.AuthenticateAsync(AuthConstants.GoogleScheme);
                if (!authResult.Succeeded)
                    return Results.Unauthorized();

                var email = authResult.Principal?.FindFirst(ClaimTypes.Email)?.Value;
                var nameIdentifier = authResult.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var displayName = authResult.Principal?.FindFirst(ClaimTypes.Name)?.Value;

                var result = await sender.Send(new OAuthAuthenticationCommand(
                    Provider: AuthConstants.GoogleScheme,
                    ProviderKey: nameIdentifier ?? string.Empty,
                    Email: email,
                    DisplayName: displayName,
                    TenantId: string.Empty)); // TenantId resolved inside handler via ITenantProvider

                return result.IsSuccess
                    ? Results.Json(ApiResponse<string>.SuccessResponse(result.Value, "Authentication successful."))
                    : Results.Json(ApiResponse.Unauthorized(result.Error),
                        statusCode: (int)HttpStatusCode.Unauthorized);
            })
            .WithName("GoogleCallback")
            .WithSummary("Google OAuth callback")
            .AllowAnonymous();

        // ---------- Current user ----------
        group.MapGet("/me", async (
                HttpContext context,
                [FromServices] ISender sender) =>
            {
                var userId = GetUserId(context);
                if (userId is null)
                    return Results.Json(ApiResponse.Unauthorized(),
                        statusCode: (int)HttpStatusCode.Unauthorized);

                var result = await sender.Send(new GetUserByIdQuery(userId));
                return result.IsSuccess
                    ? Results.Json(ApiResponse<object>.SuccessResponse(result.Value))
                    : Results.Json(ApiResponse.NotFound(result.Error),
                        statusCode: (int)HttpStatusCode.NotFound);
            })
            .WithName("GetCurrentUser")
            .WithSummary("Get current authenticated user")
            .RequireAuthorization();

        // ---------- Manage: two-factor ----------
        group.MapGet("/manage/2fa", async (
                HttpContext context,
                [FromServices] ISender sender) =>
            {
                var userId = GetUserId(context);
                if (userId is null)
                    return Results.Json(ApiResponse.Unauthorized(),
                        statusCode: (int)HttpStatusCode.Unauthorized);

                var result = await sender.Send(new GetTwoFactorStatusQuery(userId));
                return Results.Json(ApiResponse<object>.SuccessResponse(new { twoFactorEnabled = result.Value }));
            })
            .WithName("GetTwoFactorStatus")
            .WithSummary("Get two-factor authentication status")
            .RequireAuthorization();

        // ---------- Manage account: info ----------
        group.MapGet("/manage/info", async (
                HttpContext context,
                [FromServices] ISender sender) =>
            {
                var userId = GetUserId(context);
                if (userId is null)
                    return Results.Json(ApiResponse.Unauthorized(),
                        statusCode: (int)HttpStatusCode.Unauthorized);

                var result = await sender.Send(new GetManageInfoQuery(userId));
                return result.IsSuccess
                    ? Results.Json(ApiResponse<object>.SuccessResponse(result.Value))
                    : Results.Json(ApiResponse.NotFound(result.Error),
                        statusCode: (int)HttpStatusCode.NotFound);
            })
            .WithName("GetManageInfo")
            .WithSummary("Get current user account info for management")
            .RequireAuthorization();

        group.MapPost("/manage/info", async (
                [FromBody] UpdateManageInfoRequest request,
                HttpContext context,
                [FromServices] ISender sender) =>
            {
                var userId = GetUserId(context);
                if (userId is null)
                    return Results.Json(ApiResponse.Unauthorized(),
                        statusCode: (int)HttpStatusCode.Unauthorized);

                var result = await sender.Send(
                    new UpdateManageInfoCommand(userId, request.Email, request.UserName));
                return result.IsSuccess
                    ? Results.Json(ApiResponse.SuccessResponse("Account updated successfully."))
                    : Results.Json(ApiResponse.BadRequest(result.Error),
                        statusCode: (int)HttpStatusCode.BadRequest);
            })
            .WithName("UpdateManageInfo")
            .WithSummary("Update email and/or username")
            .RequireAuthorization();

        group.MapPost("/manage/delete", async (
                [FromBody] DeletePersonalDataRequest request,
                HttpContext context,
                [FromServices] ISender sender) =>
            {
                var userId = GetUserId(context);
                if (userId is null)
                    return Results.Json(ApiResponse.Unauthorized(),
                        statusCode: (int)HttpStatusCode.Unauthorized);

                var result = await sender.Send(new DeletePersonalDataCommand(userId, request.Password));
                return result.IsSuccess
                    ? Results.Json(ApiResponse.SuccessResponse("Account deleted successfully."))
                    : Results.Json(ApiResponse.BadRequest(result.Error),
                        statusCode: (int)HttpStatusCode.BadRequest);
            })
            .WithName("DeletePersonalData")
            .WithSummary("Delete account and personal data")
            .RequireAuthorization();
    }

    /// <summary>Extracts the authenticated user's ID from the JWT/cookie claims.</summary>
    private static string? GetUserId(HttpContext context) =>
        context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? context.User.FindFirst("sub")?.Value;
}

