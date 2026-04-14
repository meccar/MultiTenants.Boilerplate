using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Shared.Dtos;
using BuildingBlocks.Shared.Utilities;
using Identity.Application.Decorators;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Identity.Application.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates permissions before handling a request
/// Checks [RequirePermission] attributes on command classes
/// </summary>
public class PermissionValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : Result
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityService _identityService;
    private readonly ILogger<PermissionValidationBehavior<TRequest, TResponse>> _logger;

    public PermissionValidationBehavior(
        IHttpContextAccessor httpContextAccessor,
        IIdentityService identityService,
        ILogger<PermissionValidationBehavior<TRequest, TResponse>> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requiredPermissions = typeof(TRequest)
            .GetCustomAttributes(typeof(RequirePermissionAttribute), inherit: false)
            .Cast<RequirePermissionAttribute>()
            .Select(x => x.Permission)
            .ToList();

        // If no permissions required, proceed
        if (!requiredPermissions.Any())
        {
            return await next();
        }

        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext?.Items.TryGetValue("CurrentUser", out var currentUserObj) != true)
        {
            _logger.LogWarning("Current user not found in HttpContext");
            return (TResponse)(object)Result.Failure("Unauthorized: User not found");
        }

        var currentUser = currentUserObj as UserDto;
        if (currentUser == null)
        {
            _logger.LogWarning("Current user is not a valid UserDto");
            return (TResponse)(object)Result.Failure("Unauthorized: Invalid user");
        }

        // Get user roles
        var userRoles = await _identityService.GetUserRolesAsync(
            currentUser.Id,
            cancellationToken);

        // Check if user has any of the required permissions
        var hasPermission = await CheckPermissionsAsync(
            currentUser.Id,
            userRoles,
            requiredPermissions,
            cancellationToken);

        if (!hasPermission)
        {
            _logger.LogWarning(
                "User {UserId} denied access. Required permissions: {Permissions}",
                currentUser.Id,
                string.Join(", ", requiredPermissions));

            return (TResponse)(object)Result.Failure(
                $"Forbidden: Missing required permissions: {string.Join(", ", requiredPermissions)}");
        }

        _logger.LogInformation(
            "User {UserId} granted access to {CommandType}",
            currentUser.Id,
            typeof(TRequest).Name);

        return await next();
    }

    private async Task<bool> CheckPermissionsAsync(
        string userId,
        IReadOnlyList<string> userRoles,
        List<string> requiredPermissions,
        CancellationToken cancellationToken)
    {
        // TODO: Implement your permission checking logic
        // This could be:
        // 1. Role-based: Check if user has Admin/Manager role
        // 2. Database lookup: Query role_permissions table
        // 3. Claims-based: Check JWT claims

        // For now, example: Admin role has all permissions
        if (userRoles.Contains("Admin"))
        {
            return true;
        }

        // Example: Check permissions from database
        // var rolePermissions = await _permissionRepository.GetPermissionsByRolesAsync(userRoles);
        // return requiredPermissions.All(rp => rolePermissions.Contains(rp));

        return false;
    }
}
