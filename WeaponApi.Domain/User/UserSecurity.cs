namespace WeaponApi.Domain.User;

/// <summary>
/// Value object representing user security information.
/// Encapsulates password hash and authentication data.
/// </summary>
public sealed record UserSecurity
{
    public string PasswordHash { get; }
    public DateTime? LastLoginAt { get; }

    private UserSecurity(string passwordHash, DateTime? lastLoginAt)
    {
        PasswordHash = passwordHash;
        LastLoginAt = lastLoginAt;
    }

    public static UserSecurity Create(string passwordHash)
    {
        ValidatePasswordHash(passwordHash);
        return new UserSecurity(passwordHash, null);
    }

    public static UserSecurity Create(string passwordHash, DateTime? lastLoginAt)
    {
        ValidatePasswordHash(passwordHash);
        return new UserSecurity(passwordHash, lastLoginAt);
    }

    public UserSecurity ChangePassword(string newPasswordHash)
    {
        ValidatePasswordHash(newPasswordHash);
        return new UserSecurity(newPasswordHash, LastLoginAt);
    }

    public UserSecurity RecordLogin()
    {
        return new UserSecurity(PasswordHash, DateTime.UtcNow);
    }

    private static void ValidatePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
    }
}
