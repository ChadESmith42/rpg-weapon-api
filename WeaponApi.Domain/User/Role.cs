namespace WeaponApi.Domain.User;

/// <summary>
/// User role value object representing user permissions and access levels.
/// </summary>
public sealed record Role
{
    public string Value { get; }

    private Role(string value)
    {
        Value = value;
    }

    public static Role User => new("User");
    public static Role Admin => new("Admin");
    public static Role Application => new("Application");

    /// <summary>
    /// Checks if this role is an Application role for 3rd party integrations.
    /// </summary>
    public bool IsApplication => Value == Application.Value;

    /// <summary>
    /// Checks if this role has administrative privileges.
    /// </summary>
    public bool IsAdmin => Value == Admin.Value;

    public static Role Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Role value cannot be null or whitespace", nameof(value));

        return new Role(value.Trim());
    }

    public override string ToString() => Value;

    public static implicit operator string(Role role) => role.Value;
}
