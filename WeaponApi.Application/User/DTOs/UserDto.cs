using WeaponApi.Domain.User;

namespace WeaponApi.Application.User.DTOs;

/// <summary>
/// Data transfer object for user information in API responses.
/// UI-friendly representation excluding sensitive data like passwords.
/// DTOs are exempt from certain Object Calisthenics rules per architectural guidelines.
/// </summary>
public sealed record UserDto(
    string Id,
    string Name,
    string Username,
    string Email,
    int Age,
    IReadOnlyList<string> Roles,
    DateTime CreatedAt
)
{
    /// <summary>
    /// Creates UserDto from a domain User entity and roles.
    /// Calculates age from date of birth and excludes sensitive information.
    /// </summary>
    /// <param name="user">The domain user entity</param>
    /// <param name="roles">The user's roles</param>
    /// <returns>UserDto for API response</returns>
    public static UserDto FromDomain(Domain.User.User user, IReadOnlyList<Role> roles)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (roles == null)
            throw new ArgumentNullException(nameof(roles));

        var age = CalculateAge(user.Profile.DateOfBirth);

        return new UserDto(
            user.Id.Value.ToString(),
            user.Profile.Name,
            user.Profile.Username,
            user.Email.Value,
            age,
            roles.Select(r => r.Value.ToString()).ToList(),
            user.CreatedAt
        );
    }

    /// <summary>
    /// Creates UserDto from domain User with empty roles collection.
    /// Used when role information is not available or needed.
    /// </summary>
    /// <param name="user">The domain user entity</param>
    /// <returns>UserDto for API response</returns>
    public static UserDto FromDomain(Domain.User.User user)
    {
        return FromDomain(user, Array.Empty<Role>());
    }

    /// <summary>
    /// Calculates age from date of birth.
    /// </summary>
    /// <param name="dateOfBirth">The user's date of birth as DateOnly</param>
    /// <returns>Current age in years</returns>
    private static int CalculateAge(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dateOfBirth.Year;

        if (dateOfBirth > today.AddYears(-age))
            age--;

        return age;
    }
}
