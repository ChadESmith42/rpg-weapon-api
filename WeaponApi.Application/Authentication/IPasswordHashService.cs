namespace WeaponApi.Application.Authentication;

/// <summary>
/// Service interface for secure password hashing and verification operations.
/// Uses BCrypt algorithm for industry-standard password security.
/// </summary>
public interface IPasswordHashService
{
    /// <summary>
    /// Hashes a plain text password using BCrypt with a secure work factor.
    /// </summary>
    /// <param name="password">The plain text password to hash</param>
    /// <returns>The hashed password string</returns>
    /// <exception cref="ArgumentException">Thrown when password is null, empty, or whitespace</exception>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a plain text password against a hashed password.
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="hashedPassword">The hashed password to verify against</param>
    /// <returns>True if the password matches the hash, false otherwise</returns>
    /// <exception cref="ArgumentException">Thrown when password or hashedPassword is null, empty, or whitespace</exception>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Checks if a hashed password needs to be rehashed due to changed security requirements.
    /// This is useful for upgrading password security when work factors change.
    /// </summary>
    /// <param name="hashedPassword">The hashed password to check</param>
    /// <returns>True if the password should be rehashed, false otherwise</returns>
    bool NeedsRehash(string hashedPassword);

    /// <summary>
    /// Validates that a password meets minimum security requirements.
    /// </summary>
    /// <param name="password">The password to validate</param>
    /// <returns>Password validation result with success status and error messages</returns>
    PasswordValidationResult ValidatePasswordStrength(string password);
}
