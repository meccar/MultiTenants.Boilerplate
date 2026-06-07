using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BuildingBlocks.Shared.Configuration;
using BuildingBlocks.Shared.Dtos.Authentication;
using BuildingBlocks.Shared.Models;

namespace BuildingBlocks.Shared.Helpers;

public class JwtToken
{
    private readonly ILogger<JwtToken> _logger;
    private readonly SymmetricSecurityKey _signingKey;

    private readonly JwtOptions _jwt;

    public JwtToken(
        IConfiguration configuration,
        ILogger<JwtToken> logger
    )
    {
        _logger = logger;
        _jwt = configuration.GetSection<JwtOptions>("Jwt");

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
    }

    public Task<TokenDto> GenerateJwtTokenAsync(
        GenerateTokenRequestModel request)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.UniqueName, request.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("security_stamp", request.SecurityStamp),
            };

            if (!string.IsNullOrWhiteSpace(request.TenantId))
                claims.Add(new Claim("tenant_id", request.TenantId));

            claims.AddRange(request.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var credentials = new SigningCredentials(
                _signingKey,
                SecurityAlgorithms.HmacSha256);
            var now = DateTime.Now;
            var accessExpiry = now.AddMinutes(_jwt.AccessExpiry);
            var refreshExpiry = now.AddMinutes(_jwt.RefreshExpiry);

            var accessKey = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: accessExpiry,
                signingCredentials: credentials);
            
            var refreshKey = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                notBefore: now,
                expires: refreshExpiry,
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(accessKey);
            var refreshToken = new JwtSecurityTokenHandler().WriteToken(refreshKey);
            
            return Task.FromResult(new TokenDto(
                AccessToken: accessToken,
                RefreshToken: refreshToken
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token for user {UserName}", request.UserName);
            throw;
        }
    }

    public TokenValidationResult ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return TokenValidationResult.Failure("Token cannot be null or empty");
        
        var handler = new JwtSecurityTokenHandler();
        
        if (!handler.CanReadToken(token))
            return TokenValidationResult.Failure("Token format is invalid");
        
        try
        {
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validated);

            if (validated is not JwtSecurityToken jwt)
                return TokenValidationResult.Failure("Token is not a valid JWT");
            
            return TokenValidationResult.Success(jwt);
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning("Token expired at {ExpiredAt}", ex.Expires);
            return TokenValidationResult.Failure("Token has expired");
        }
        catch (SecurityTokenNotYetValidException ex)
        {
            _logger.LogWarning("Token not valid before {NotBefore}", ex.NotBefore);
            return TokenValidationResult.Failure("Token is not yet valid");
        }
        catch (SecurityTokenInvalidIssuerException)
        {
            _logger.LogWarning("Token has invalid issuer");
            return TokenValidationResult.Failure("Token issuer is invalid");
        }
        catch (SecurityTokenInvalidAudienceException)
        {
            _logger.LogWarning("Token has invalid audience");
            return TokenValidationResult.Failure("Token audience is invalid");
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            _logger.LogWarning("Token signature is invalid");
            return TokenValidationResult.Failure("Token signature is invalid");
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Token validation failed: {Message}", ex.Message);
            return TokenValidationResult.Failure("Token is invalid");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Token validation failed");
            return TokenValidationResult.Failure("Token is invalid");
        }
    }
    
    private static string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }
}

public sealed class TokenValidationResult
{
    public bool IsValid { get; init; }
    public string? Error { get; init; }

    // public string? UserId { get; init; }
    public string? Username { get; init; }
    public string? TenantId { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public long? ExpiresAt { get; init; }

    public static TokenValidationResult Success(JwtSecurityToken jwt) => new()
    {
        IsValid = true,
        // UserId = jwt.Subject,
        Username = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value,
        TenantId = jwt.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value,
        Roles = jwt.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList(),
        ExpiresAt = jwt.ValidTo == DateTime.MinValue ? null : jwt.ValidTo.Ticks,
    };

    public static TokenValidationResult Failure(string error) => new()
    {
        IsValid = false,
        Error = error,
    };
}
