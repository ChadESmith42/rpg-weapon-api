using System.Text.RegularExpressions;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using WeaponApi.Application.Authentication;

namespace WeaponApi.Infrastructure.Authentication;

/// <summary>
/// BCrypt-based password hashing service for secure password storage and verification.
/// Implements industry-standard password security with configurable work factors.
/// </summary>
public sealed class PasswordHashService : IPasswordHashService
{
    private readonly IConfiguration configuration;
    private readonly int workFactor;

    public PasswordHashService(IConfiguration configuration)
    {
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        this.workFactor = GetWorkFactorFromConfiguration();
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null, empty, or whitespace", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null, empty, or whitespace", nameof(password));

        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be null, empty, or whitespace", nameof(hashedPassword));

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (SaltParseException)
        {
            // Invalid hash format
            return false;
        }
    }

    public bool NeedsRehash(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be null, empty, or whitespace", nameof(hashedPassword));

        try
        {
            var currentWorkFactor = ExtractWorkFactorFromHash(hashedPassword);
            return currentWorkFactor < workFactor;
        }
        catch
        {
            // If we can't parse the hash, it should be rehashed
            return true;
        }
    }

    public PasswordValidationResult ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password))
            return PasswordValidationResult.Invalid("Password is required");

        var errors = new List<string>();

        // Check minimum length
        var minLength = GetMinimumPasswordLength();
        if (password.Length < minLength)
            errors.Add($"Password must be at least {minLength} characters long");

        // Check for uppercase letter
        if (!ContainsUppercase(password))
            errors.Add("Password must contain at least one uppercase letter");

        // Check for lowercase letter
        if (!ContainsLowercase(password))
            errors.Add("Password must contain at least one lowercase letter");

        // Check for digit
        if (!ContainsDigit(password))
            errors.Add("Password must contain at least one digit");

        // Check for special character
        if (!ContainsSpecialCharacter(password))
            errors.Add("Password must contain at least one special character (!@#$%^&*)");

        // Check for whitespace
        if (ContainsWhitespace(password))
            errors.Add("Password cannot contain whitespace characters");

        // Check for common weak patterns
        if (IsCommonWeakPassword(password))
            errors.Add("Password is too common or weak");

        return errors.Count == 0
            ? PasswordValidationResult.Valid()
            : PasswordValidationResult.Invalid(errors);
    }

    private int GetWorkFactorFromConfiguration()
    {
        var configValue = configuration["PasswordHashing:WorkFactor"];

        if (int.TryParse(configValue, out var factor))
        {
            // Ensure work factor is within secure range (minimum 10, maximum 15 for performance)
            return Math.Max(10, Math.Min(15, factor));
        }

        // Default to 12 if not configured (good balance of security and performance)
        return 12;
    }

    private int GetMinimumPasswordLength()
    {
        var configValue = configuration["PasswordHashing:MinimumLength"];

        if (int.TryParse(configValue, out var length))
        {
            // Ensure minimum length is at least 8 characters
            return Math.Max(8, length);
        }

        // Default to 8 characters minimum
        return 8;
    }

    private static int ExtractWorkFactorFromHash(string hashedPassword)
    {
        // BCrypt hash format: $2a$rounds$salt+hash
        // Extract the rounds (work factor) from the hash
        var parts = hashedPassword.Split('$');
        if (parts.Length >= 3 && int.TryParse(parts[2], out var rounds))
        {
            return rounds;
        }

        throw new ArgumentException("Invalid BCrypt hash format");
    }

    private static bool ContainsUppercase(string password) =>
        password.Any(char.IsUpper);

    private static bool ContainsLowercase(string password) =>
        password.Any(char.IsLower);

    private static bool ContainsDigit(string password) =>
        password.Any(char.IsDigit);

    private static bool ContainsSpecialCharacter(string password) =>
        password.Any(c => "!@#$%^&*()_+-=[]{}|;:,.<>?".Contains(c));

    private static bool ContainsWhitespace(string password) =>
        password.Any(char.IsWhiteSpace);

    private static bool IsCommonWeakPassword(string password)
    {
        var lowercasePassword = password.ToLowerInvariant();

        // Common weak password patterns
        var weakPatterns = new[]
        {
            "password", "123456", "qwerty", "abc123", "letmein",
            "welcome", "monkey", "dragon", "master", "hello",
            "login", "admin", "test", "guest", "user"
        };

        // Check if password is a common weak password
        if (weakPatterns.Any(pattern => lowercasePassword.Contains(pattern)))
            return true;

        // Check for simple patterns like "12345678" or "abcdefgh"
        if (IsSequentialPattern(password))
            return true;

        // Check for repeated characters like "aaaaaaaa"
        if (IsRepeatedCharacters(password))
            return true;

        return false;
    }

    private static bool IsSequentialPattern(string password)
    {
        if (password.Length < 4) return false;

        // Check for ascending sequences
        for (var i = 0; i < password.Length - 3; i++)
        {
            var isAscending = true;
            for (var j = 1; j < 4; j++)
            {
                if (password[i + j] != password[i] + j)
                {
                    isAscending = false;
                    break;
                }
            }
            if (isAscending) return true;
        }

        // Check for descending sequences
        for (var i = 0; i < password.Length - 3; i++)
        {
            var isDescending = true;
            for (var j = 1; j < 4; j++)
            {
                if (password[i + j] != password[i] - j)
                {
                    isDescending = false;
                    break;
                }
            }
            if (isDescending) return true;
        }

        return false;
    }

    private static bool IsRepeatedCharacters(string password)
    {
        if (password.Length < 4) return false;

        // Check if more than 50% of characters are the same
        var characterCounts = password.GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());

        var maxCount = characterCounts.Values.Max();
        return (double)maxCount / password.Length > 0.5;
    }
}
