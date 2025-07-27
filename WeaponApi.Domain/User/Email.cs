namespace WeaponApi.Domain.User;

/// <summary>
/// Value object representing an email address.
/// Ensures email validation and immutability.
/// </summary>
public sealed record Email
{
    public string Value { get; }

    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        if (!IsValidEmail(value))
            throw new ArgumentException("Invalid email format", nameof(value));

        Value = value.ToLowerInvariant();
    }

    public static Email Create(string emailAddress) => new(emailAddress);

    public static implicit operator string(Email email) => email.Value;

    public override string ToString() => Value;

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
