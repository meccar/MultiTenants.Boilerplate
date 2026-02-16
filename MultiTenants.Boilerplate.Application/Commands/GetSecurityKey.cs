using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MultiTenants.Boilerplate.Application.Commands
{
    public class GetSecurityKey
    {

    public static string MaskInput(string input)
        => input.Length > 3
            ? input[..3] + "***" 
            : "***";

  public async Task<string> GenerateJwtTokenAsync(
    IdentityUser user,
    IList<string> roles,
    TenantInfo tenant
  ){
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

        public SymmetricSecurityKey GetSecurityKey(string key)
            => _cachedKey ??= new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));
    }
}