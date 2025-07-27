using WeaponApi.Application.Common;
using WeaponApi.Domain.User;

namespace WeaponApi.Application.User;

/// <summary>
/// Repository interface for User aggregate operations.
/// Provides business-oriented methods for user persistence and retrieval.
/// </summary>
public interface IUserRepository : IRepository<Domain.User.User, UserId>
{
    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<Domain.User.User?> FindByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a user by their username.
    /// </summary>
    /// <param name="username">The username</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<Domain.User.User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified email address exists.
    /// </summary>
    /// <param name="email">The email address to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if a user with the email exists, false otherwise</returns>
    Task<bool> EmailExistsAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified username exists.
    /// </summary>
    /// <param name="username">The username to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if a user with the username exists, false otherwise</returns>
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
}
