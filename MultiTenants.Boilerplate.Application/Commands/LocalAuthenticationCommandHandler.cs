using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MultiTenants.Boilerplate.Shared.Constants.Errors;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands;

public class LocalAuthenticationCommandHandler : IRequestHandler<LocalAuthenticationCommand, Result<string>>
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IConfiguration _configuration;
  private readonly ILogger<LocalAuthenticationCommandHandler> _logger;
  private SymmetricSecurityKey? _cachedKey;


  public LocalAuthenticationCommandHandler(
    UserManager<IdentityUser> userManager,
    IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration,
    ILogger<LocalAuthenticationCommandHandler> logger
  )
    {
    _userManager = userManager;
    _tenantContextAccessor = tenantContextAccessor;
    _httpContextAccessor = httpContextAccessor;
    _configuration = configuration;
    _logger = logger;
    }

  public async Task<Result<string>> Handle(LocalAuthenticationCommand request, CancellationToken cancellationToken)
  {
    var tenant = _tenantContextAccessor.MultiTenantContext.TenantInfo;
    if (tenant is null)
    { 
      _logger.LogWarning("Login attempted without tenant context");
      return Result<string>.Failure("Tenant context not found");
    }

    var user = await GetUserAsync(request.UserName);
    if(user is null)
    {
      _logger.LogWarning("Login failed: User {UserName} not found in tenant {TenantId}", 
          MaskInput(request.UserName), tenant.Id);
      return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);
    }

    if (!await _userManager.IsEmailConfirmedAsync(user))
    { 
      _logger.LogWarning("Login failed: User {UserId} email is not confirmed in tenant {TenantId}", 
            user.Id, tenant.Id);
      return Result<string>.Failure("User email is not confirmed");
    }

    var result = await _signInManager.PasswordSignInAsync(
        user.UserName!,
        request.Password,
        isPersistent: request.IsPersistent,
        lockoutOnFailure: true
    );

    if (result.IsLockedOut)
        return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

    if (result.RequiresTwoFactor)
        return Result<string>.Failure("2FA_REQUIRED");

    if (!result.Succeeded)
        return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

    var token = await GenerateJwtTokenAsync(user, tenant);

    _logger.LogInformation("User {UserId} successfully authenticated in tenant {TenantId}", 
        user.Id, tenant.Id);

    return Result<string>.Success(token);
  }

  private async Task<IdentityUser?> GetUserAsync(string userName)
  {
    return await _userManager.FindByNameAsync(userName)
      ?? await _userManager.FindByEmailAsync(userName);
  }

  private async Task<string> GenerateJwtTokenAsync(
    IdentityUser user,
    TenantInfo tenant)
  {
    var jwtKey = _configuration["Jwt:Key"];
    var jwtIssuer = _configuration["Jwt:Issuer"];
    var jwtAudience = _configuration["Jwt:Audience"];

    if (string.IsNullOrEmpty(jwtKey) 
            || string.IsNullOrEmpty(jwtIssuer) 
            || string.IsNullOrEmpty(jwtAudience))
    {
      _logger.LogError("JWT configuration is incomplete or missing");
       throw new InvalidOperationException("Jwt configuration is incomplete");
    }

    var roles = await _userManager.GetRolesAsync(user);
    var claims = new List<Claim>
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
      new Claim("tenant_id", tenant.Id ?? ""),
    };

    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        
    var key = GetSecurityKey(jwtKey);

    var credentials = new SigningCredentials(
      key,
      SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: jwtIssuer,
      audience: jwtAudience,
      claims: claims,
      expires: DateTime.UtcNow.AddHours(2),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

    private SymmetricSecurityKey GetSecurityKey(string key)
    {
        return _cachedKey ??= new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));
    }

    private static string MaskInput(string input)
    {
        return input.Length > 3
            ? input[..3] + "***" : "***";
    }

}
