using System.Net;

namespace WeaponApi.Application.Authentication;

/// <summary>
/// Result type for JWT token validation operations.
/// Provides structured error handling with HTTP status codes.
/// </summary>
public sealed class TokenValidationResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public HttpStatusCode? ErrorStatusCode { get; }

    private TokenValidationResult(bool isSuccess, string? errorMessage = null, HttpStatusCode? errorStatusCode = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorStatusCode = errorStatusCode;
    }

    public static TokenValidationResult Success() => new(true);

    public static TokenValidationResult Failure(string errorMessage, HttpStatusCode statusCode) =>
        new(false, errorMessage, statusCode);

    public static TokenValidationResult InvalidToken() =>
        new(false, "Invalid or malformed token", HttpStatusCode.Unauthorized);

    public static TokenValidationResult ExpiredToken() =>
        new(false, "Token has expired", HttpStatusCode.Unauthorized);

    public static TokenValidationResult MissingToken() =>
        new(false, "Token is required", HttpStatusCode.Unauthorized);
}

/// <summary>
/// Result type for JWT token data extraction operations.
/// Provides structured error handling with extracted values.
/// </summary>
/// <typeparam name="T">The type of data being extracted from the token</typeparam>
public sealed class TokenExtractionResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? ErrorMessage { get; }
    public HttpStatusCode? ErrorStatusCode { get; }

    private TokenExtractionResult(bool isSuccess, T? value = default, string? errorMessage = null, HttpStatusCode? errorStatusCode = null)
    {
        IsSuccess = isSuccess;
        Value = value;
        ErrorMessage = errorMessage;
        ErrorStatusCode = errorStatusCode;
    }

    public static TokenExtractionResult<T> Success(T value) => new(true, value);

    public static TokenExtractionResult<T> Failure(string errorMessage, HttpStatusCode statusCode) =>
        new(false, default, errorMessage, statusCode);

    public static TokenExtractionResult<T> InvalidToken() =>
        new(false, default, "Invalid or malformed token", HttpStatusCode.Unauthorized);

    public static TokenExtractionResult<T> ExpiredToken() =>
        new(false, default, "Token has expired", HttpStatusCode.Unauthorized);

    public static TokenExtractionResult<T> MissingClaim(string claimName) =>
        new(false, default, $"Required claim '{claimName}' not found in token", HttpStatusCode.Unauthorized);
}
