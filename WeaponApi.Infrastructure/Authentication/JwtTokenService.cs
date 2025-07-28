using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WeaponApi.Application.Authentication;
using WeaponApi.Domain.User;
using AppTokenValidationResult = WeaponApi.Application.Authentication.TokenValidationResult;

namespace WeaponApi.Infrastructure.Authentication;

/// <summary>
/// JWT token service implementation for user authentication and authorization.
/// Provides secure token generation with role-based expiration and comprehensive validation.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration configuration;
    private readonly JwtSecurityTokenHandler tokenHandler;

    public JwtTokenService(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.tokenHandler = new JwtSecurityTokenHandler();
    }

    public string GenerateAccessToken(User user, IReadOnlyList<Role> roles)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (roles == null) throw new ArgumentNullException(nameof(roles));

        var key = GetSigningKey();
        var expiration = CalculateTokenExpiration(roles);

        var claims = CreateTokenClaims(user, roles);
        var tokenDescriptor = CreateTokenDescriptor(claims, key, expiration);

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken(UserId userId)
    {
        if (userId == null) throw new ArgumentNullException(nameof(userId));

        var key = GetSigningKey();
        var expiration = DateTime.UtcNow.AddDays(30); // 30-day refresh token

        var claims = new[]
        {
            new Claim("sub", userId.Value.ToString()),
            new Claim("type", "refresh"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = CreateTokenDescriptor(claims, key, expiration);
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public AppTokenValidationResult ValidateToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return AppTokenValidationResult.MissingToken();

        try
        {
            var validationParameters = CreateValidationParameters();
            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            return AppTokenValidationResult.Success();
        }
        catch (SecurityTokenExpiredException)
        {
            return AppTokenValidationResult.ExpiredToken();
        }
        catch (SecurityTokenException)
        {
            return AppTokenValidationResult.InvalidToken();
        }
        catch (Exception)
        {
            return AppTokenValidationResult.InvalidToken();
        }
    }

    public TokenExtractionResult<UserId> GetUserIdFromToken(string token)
    {
        var claimsPrincipal = ExtractClaimsFromToken(token);
        if (claimsPrincipal == null)
            return TokenExtractionResult<UserId>.InvalidToken();

        // Try both "sub" and the full .NET claim type for nameidentifier
        var userIdClaim = claimsPrincipal.FindFirst("sub") ??
                         claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
            return TokenExtractionResult<UserId>.MissingClaim("sub");

        if (Guid.TryParse(userIdClaim.Value, out var guid))
        {
            return TokenExtractionResult<UserId>.Success(UserId.Create(guid));
        }

        return TokenExtractionResult<UserId>.InvalidToken();
    }

    public TokenExtractionResult<Email> GetEmailFromToken(string token)
    {
        var claimsPrincipal = ExtractClaimsFromToken(token);
        if (claimsPrincipal == null)
            return TokenExtractionResult<Email>.InvalidToken();

        var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);
        if (emailClaim == null || string.IsNullOrWhiteSpace(emailClaim.Value))
            return TokenExtractionResult<Email>.MissingClaim("email");

        try
        {
            var email = Email.Create(emailClaim.Value);
            return TokenExtractionResult<Email>.Success(email);
        }
        catch (ArgumentException)
        {
            return TokenExtractionResult<Email>.InvalidToken();
        }
    }

    public TokenExtractionResult<IReadOnlyList<Role>> GetRolesFromToken(string token)
    {
        var claimsPrincipal = ExtractClaimsFromToken(token);
        if (claimsPrincipal == null)
            return TokenExtractionResult<IReadOnlyList<Role>>.InvalidToken();

        var roleClaims = claimsPrincipal.FindAll(ClaimTypes.Role);
        if (!roleClaims.Any())
            return TokenExtractionResult<IReadOnlyList<Role>>.MissingClaim("role");

        try
        {
            var roles = roleClaims
                .Select(claim => Role.Create(claim.Value))
                .ToList()
                .AsReadOnly();

            return TokenExtractionResult<IReadOnlyList<Role>>.Success(roles);
        }
        catch (ArgumentException)
        {
            return TokenExtractionResult<IReadOnlyList<Role>>.InvalidToken();
        }
    }

    public bool IsTokenExpired(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        try
        {
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.ValidTo < DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Invalid token format", ex);
        }
    }

    public TimeSpan GetTokenTimeToExpiration(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        try
        {
            var jsonToken = tokenHandler.ReadJwtToken(token);
            var timeToExpiration = jsonToken.ValidTo - DateTime.UtcNow;
            return timeToExpiration > TimeSpan.Zero ? timeToExpiration : TimeSpan.Zero;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Invalid token format", ex);
        }
    }

    private SymmetricSecurityKey GetSigningKey()
    {
        var secretKey = configuration["Jwt:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey))
            throw new InvalidOperationException("JWT secret key is not configured");

        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        return new SymmetricSecurityKey(keyBytes)
        {
            KeyId = "WeaponApiKey"
        };
    }

    private DateTime CalculateTokenExpiration(IReadOnlyList<Role> roles)
    {
        var hasApplicationRole = roles.Any(role => role.IsApplication);

        // Application roles get 24-hour tokens, others get 3-hour tokens
        var expirationHours = hasApplicationRole ? 24 : 3;
        return DateTime.UtcNow.AddHours(expirationHours);
    }

    private static Claim[] CreateTokenClaims(Domain.User.User user, IReadOnlyList<Role> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
            new(ClaimTypes.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Value)));

        return claims.ToArray();
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(Claim[] claims, SymmetricSecurityKey key, DateTime expiration)
    {
        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };
    }

    private TokenValidationParameters CreateValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = GetSigningKey(),
            ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes clock skew
        };
    }

    private ClaimsPrincipal? ExtractClaimsFromToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        try
        {
            var validationParameters = CreateValidationParameters();
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
