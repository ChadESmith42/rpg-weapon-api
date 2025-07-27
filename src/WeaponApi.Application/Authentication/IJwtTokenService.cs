using WeaponApi.Domain.User;

namespace WeaponApi.Application.Authentication;

/// <summary>
/// Service interface for JWT token operations.
/// Handles token generation, validation, and user claim extraction.
/// Uses industry-standard JWT algorithms with 180-minute expiration.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT access token for the authenticated user.
    /// Token expires after 180 minutes for User/Admin roles or 24 hours for Application role.
    /// Includes UserId and Roles claims.
    /// </summary>
    /// <param name="user">The authenticated user</param>
    /// <param name="roles">The user's roles</param>
    /// <returns>JWT access token as string</returns>
    string GenerateAccessToken(User user, IReadOnlyList<Role> roles);

    /// <summary>
    /// Generates a stateless refresh token for token renewal.
    /// </summary>
    /// <param name="userId">The user identifier</param>
    /// <returns>Refresh token as string</returns>
    string GenerateRefreshToken(UserId userId);

    /// <summary>
    /// Validates a JWT token and returns validation result.
    /// </summary>
    /// <param name="token">The JWT token to validate</param>
    /// <returns>Token validation result with user information or error details</returns>
    TokenValidationResult ValidateToken(string token);

    /// <summary>
    /// Extracts the user ID from a valid JWT token.
    /// </summary>
    /// <param name="token">The JWT token</param>
    /// <returns>Token extraction result with UserId or error details</returns>
    TokenExtractionResult<UserId> GetUserIdFromToken(string token);

    /// <summary>
    /// Extracts the user email from a valid JWT token.
    /// </summary>
    /// <param name="token">The JWT token</param>
    /// <returns>Token extraction result with Email or error details</returns>
    TokenExtractionResult<Email> GetEmailFromToken(string token);

    /// <summary>
    /// Extracts the user roles from a valid JWT token.
    /// </summary>
    /// <param name="token">The JWT token</param>
    /// <returns>Token extraction result with roles or error details</returns>
    TokenExtractionResult<IReadOnlyList<Role>> GetRolesFromToken(string token);

    /// <summary>
    /// Checks if a JWT token is expired.
    /// </summary>
    /// <param name="token">The JWT token to check</param>
    /// <returns>True if token is expired, false if valid, throws exception if malformed</returns>
    bool IsTokenExpired(string token);

    /// <summary>
    /// Gets the remaining time before token expiration.
    /// </summary>
    /// <param name="token">The JWT token</param>
    /// <returns>TimeSpan until expiration</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when token is invalid or malformed</exception>
    TimeSpan GetTokenTimeToExpiration(string token);
}
