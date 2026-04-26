using BuildingBlocks.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BuildingBlocks.Shared.Helpers;

public class JwtToken
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtToken> _logger;
    private readonly SymmetricSecurityKey _signingKey;

    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _expirationMinutes;

    public JwtToken(
        IConfiguration configuration,
        ILogger<JwtToken> logger
    )
    {
        _configuration = configuration;
        _logger = logger;

        _jwtSecret = _configuration["Jwt:Secret"] ?? "";
        _jwtIssuer = _configuration["Jwt:Issuer"] ?? "";
        _jwtAudience = _configuration["Jwt:Audience"] ?? "";
        
        if (!int.TryParse(
            _configuration["Jwt:ExpirationMinutes"],
            out _expirationMinutes))
        {
            _expirationMinutes = 120;
        }

        ValidateJwtConfiguration();

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
    }

    private void ValidateJwtConfiguration()
    {
        if (string.IsNullOrEmpty(_jwtSecret)
            || string.IsNullOrEmpty(_jwtIssuer)
            || string.IsNullOrEmpty(_jwtAudience))
        {
            _logger.LogError("JWT configuration is incomplete or missing");
            throw new InvalidOperationException("Jwt configuration is incomplete");
        }

        if (_expirationMinutes <= 0)
        {
            _logger.LogError("JWT expiration minutes must be greater than zero");
            throw new InvalidOperationException("Jwt expiration time is invalid");
        }
    }

    public Task<string> GenerateJwtTokenAsync(
        UserDto user,
        IList<string> roles,
        string tenantId)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return GenerateJwtTokenAsync(user.UserName, roles, tenantId);
    }

    public Task<string> GenerateJwtTokenAsync(
        string? userName,
        IList<string> roles,
        string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be null or empty", nameof(tenantId));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, userName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("tenant_id", tenantId)
            };

            claims.AddRange(
                roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var credentials = new SigningCredentials(
                _signingKey,
                SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _jwtIssuer,
                audience: _jwtAudience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_expirationMinutes),
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return Task.FromResult(tokenString);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate JWT token for user {UserId}", userName);
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
                ValidIssuer = _jwtIssuer,
                ValidateAudience = true,
                ValidAudience = _jwtAudience,
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
}

public sealed class TokenValidationResult
{
    public bool IsValid { get; init; }
    public string? Error { get; init; }

    // public string? UserId { get; init; }
    public string? Username { get; init; }
    public string? TenantId { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];
    public DateTime? ExpiresAt { get; init; }

    public static TokenValidationResult Success(JwtSecurityToken jwt) => new()
    {
        IsValid = true,
        // UserId = jwt.Subject,
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
