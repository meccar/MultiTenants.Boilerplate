using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Finbuckle.MultiTenant.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MultiTenants.Boilerplate.Shared.Constants.Errors;
using MultiTenants.Boilerplate.Shared.Utilities;

namespace MultiTenants.Boilerplate.Application.Commands;

public class LocalAuthenticationCommandHandler : IRequestHandler<LocalAuthenticationCommand, Result<string>>
{
  private readonly UserManager<IdentityUser> _userManager;
  private readonly IMultiTenantContextAccessor<TenantInfo> _tenantContextAccessor;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly IConfiguration _configuration;

  public LocalAuthenticationCommandHandler(
    UserManager<IdentityUser> userManager,
    IMultiTenantContextAccessor<TenantInfo> tenantContextAccessor,
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration)
  {
    _userManager = userManager;
    _tenantContextAccessor = tenantContextAccessor;
    _httpContextAccessor = httpContextAccessor;
    _configuration = configuration;
  }

  public async Task<Result<string>> Handle(LocalAuthenticationCommand request, CancellationToken cancellationToken)
  {
    var tenant = _tenantContextAccessor.MultiTenantContext.TenantInfo;
    if (tenant is null)
      return Result<string>.Failure("Tenant context not found");

    var user = await GetUserAsync(request.UserName);
    if(user is null)
      return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

    if (!await _userManager.IsEmailConfirmedAsync(user))
      return Result<string>.Failure("User email is not confirmed");

    if (!await _userManager.CheckPasswordAsync(user, request.Password))
      return Result<string>.Failure(AuthenticationErrors.InvalidCredentials);

    var token = await GenerateJwtTokenAsync(user, tenant);

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
    var roles = await _userManager.GetRolesAsync(user);
    var claims = new List<Claim>
    {
      new Claim(JwtRegisteredClaimNames.Sub, user.Id),
      new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
      new Claim("tenant_id", tenant.Id ?? ""),
    };

    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var key = new SymmetricSecurityKey(
      Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

    var credentials = new SigningCredentials(
      key,
      SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
      issuer: _configuration["Jwt:Issuer"],
      audience: _configuration["Jwt:Audience"],
      claims: claims,
      expires: DateTime.UtcNow.AddHours(2),
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

}
