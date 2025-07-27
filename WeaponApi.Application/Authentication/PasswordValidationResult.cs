namespace WeaponApi.Application.Authentication;

/// <summary>
/// Result type for password strength validation operations.
/// Provides structured validation feedback for password requirements.
/// </summary>
public sealed class PasswordValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<string> ErrorMessages { get; }

    private PasswordValidationResult(bool isValid, IReadOnlyList<string> errorMessages)
    {
        IsValid = isValid;
        ErrorMessages = errorMessages;
    }

    public static PasswordValidationResult Valid() => new(true, Array.Empty<string>());

    public static PasswordValidationResult Invalid(params string[] errorMessages) =>
        new(false, errorMessages.ToList().AsReadOnly());

    public static PasswordValidationResult Invalid(IEnumerable<string> errorMessages) =>
        new(false, errorMessages.ToList().AsReadOnly());

    /// <summary>
    /// Gets a single error message string for display purposes.
    /// </summary>
    public string GetErrorMessage() => string.Join("; ", ErrorMessages);

    /// <summary>
    /// Common validation results for typical password requirements.
    /// </summary>
    public static class Common
    {
        public static PasswordValidationResult TooShort(int minLength) =>
            Invalid($"Password must be at least {minLength} characters long");

        public static PasswordValidationResult MissingUppercase() =>
            Invalid("Password must contain at least one uppercase letter");

        public static PasswordValidationResult MissingLowercase() =>
            Invalid("Password must contain at least one lowercase letter");

        public static PasswordValidationResult MissingDigit() =>
            Invalid("Password must contain at least one digit");

        public static PasswordValidationResult MissingSpecialCharacter() =>
            Invalid("Password must contain at least one special character (!@#$%^&*)");

        public static PasswordValidationResult ContainsWhitespace() =>
            Invalid("Password cannot contain whitespace characters");

        public static PasswordValidationResult TooWeak() =>
            Invalid("Password does not meet minimum security requirements");
    }
}
