using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using BuildingBlocks.Shared.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuildingBlocks.Application.Helpers;

public class JwtToken
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtToken> _logger;
    private readonly SymmetricSecurityKey _signingKey;

    private readonly string _jwtKey;
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    public JwtToken(
        IConfiguration configuration,
        ILogger<JwtToken> logger
    )
    {
        _configuration = configuration;
        _logger = logger;

        _jwtKey = _configuration["Jwt:Key"] ?? "";
        _jwtSecret = _configuration["Jwt:Secret"] ?? "";
        _jwtIssuer = _configuration["Jwt:Issuer"] ?? "";
        _jwtAudience = _configuration["Jwt:Audience"] ?? "";

        ValidateJwtConfiguration();

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
    }

    private void ValidateJwtConfiguration()
    {
        if (string.IsNullOrEmpty(_jwtKey)
            || string.IsNullOrEmpty(_jwtSecret)
            || string.IsNullOrEmpty(_jwtIssuer)
            || string.IsNullOrEmpty(_jwtAudience)
        )
        {
            _logger.LogError("JWT configuration is incomplete or missing");
            throw new InvalidOperationException("Jwt configuration is incomplete");
        }
    }

    public Task<string> GenerateJwtTokenAsync(
        IdentityUser user,
        IList<string> roles,
        string tenantId)
        => GenerateJwtTokenAsync(user.Id, user.UserName, roles, tenantId);

    public Task<string> GenerateJwtTokenAsync(
        UserDto user,
        IList<string> roles,
        string tenantId)
        => GenerateJwtTokenAsync(user.Id, user.UserName, roles, tenantId);

    private Task<string> GenerateJwtTokenAsync(
        string userId,
        string? userName,
        IList<string> roles,
        string tenantId)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, userName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("tenant_id", tenantId)
        };

        claims.AddRange(
            roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(
            _signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return Task.FromResult(
            new JwtSecurityTokenHandler().WriteToken(token));
    }

    public TokenValidationResult ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validated);

            var jwt = (JwtSecurityToken)validated;
            return TokenValidationResult.Success(jwt);
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning("Token expired at {ExpiredAt}", ex.Expires);
            return TokenValidationResult.Failure("Token has expired");
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            _logger.LogWarning("Token signature is invalid");
            return TokenValidationResult.Failure("Token signature is invalid");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return TokenValidationResult.Failure("Token is invalid");
        }
    }
}

public sealed class TokenValidationResult
{
    public bool IsValid { get; init; }
    public string? Error { get; init; }

    public string? UserId { get; init; }
    public string? Username { get; init; }
    public string? TenantId { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public DateTime? ExpiresAt { get; init; }

    public static TokenValidationResult Success(JwtSecurityToken jwt) => new()
    {
        IsValid = true,
        UserId = jwt.Subject,
        Username = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value,
        TenantId = jwt.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value,
        Roles = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
        ExpiresAt = jwt.ValidTo == DateTime.MinValue ? null : jwt.ValidTo,
    };

    public static TokenValidationResult Failure(string error) => new()
    {
        IsValid = false,
        Error = error,
    };
}
