namespace WeaponApi.Domain.User;

/// <summary>
/// Interface for password hashing operations.
/// Abstraction for password security implementations.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain text password.
    /// </summary>
    /// <param name="password">Plain text password to hash</param>
    /// <returns>Hashed password</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against its hash.
    /// </summary>
    /// <param name="password">Plain text password to verify</param>
    /// <param name="hashedPassword">Stored password hash</param>
    /// <returns>True if password matches the hash</returns>
    bool VerifyPassword(string password, string hashedPassword);
}
