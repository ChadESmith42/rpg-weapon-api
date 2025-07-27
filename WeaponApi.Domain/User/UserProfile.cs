namespace WeaponApi.Domain.User;

/// <summary>
/// Value object representing user profile information.
/// Encapsulates username, name, and date of birth.
/// </summary>
public sealed record UserProfile
{
    public string Username { get; }
    public string Name { get; }
    public DateOnly DateOfBirth { get; }

    private UserProfile(string username, string name, DateOnly dateOfBirth)
    {
        Username = username;
        Name = name;
        DateOfBirth = dateOfBirth;
    }

    public static UserProfile Create(string username, string name, DateOnly dateOfBirth)
    {
        ValidateUsername(username);
        ValidateName(name);
        return new UserProfile(username, name, dateOfBirth);
    }

    public UserProfile UpdateUsername(string newUsername)
    {
        ValidateUsername(newUsername);
        return new UserProfile(newUsername, Name, DateOfBirth);
    }

    public UserProfile UpdateName(string newName)
    {
        ValidateName(newName);
        return new UserProfile(Username, newName, DateOfBirth);
    }

    public UserProfile UpdateDateOfBirth(DateOnly newDateOfBirth)
    {
        return new UserProfile(Username, Name, newDateOfBirth);
    }

    public int GetAge()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - DateOfBirth.Year;

        if (DateOfBirth > today.AddYears(-age))
            age--;

        return age;
    }

    private static void ValidateUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (username.Length < 3)
            throw new ArgumentException("Username must be at least 3 characters", nameof(username));

        if (username.Length > 50)
            throw new ArgumentException("Username cannot exceed 50 characters", nameof(username));

        if (!IsValidUsernameFormat(username))
            throw new ArgumentException("Username can only contain letters, numbers, underscores, and hyphens", nameof(username));
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (name.Length > 100)
            throw new ArgumentException("Name cannot exceed 100 characters", nameof(name));
    }

    private static bool IsValidUsernameFormat(string username)
    {
        return username.All(c => char.IsLetterOrDigit(c) || c == '_' || c == '-');
    }
}
