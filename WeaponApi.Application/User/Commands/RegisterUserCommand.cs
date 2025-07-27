using MediatR;
using WeaponApi.Domain.User;

namespace WeaponApi.Application.User.Commands;

/// <summary>
/// Command to register a new user in the system.
/// Follows MediatR CQRS pattern for user registration handling.
/// </summary>
public sealed record RegisterUserCommand(
    string Username,
    string Email,
    string Password,
    string ConfirmPassword
) : IRequest<RegisterUserCommandResult>;

/// <summary>
/// Result type for RegisterUserCommand execution.
/// Provides structured response with user data or error information.
/// </summary>
public sealed class RegisterUserCommandResult
{
    public bool IsSuccess { get; }
    public UserId? UserId { get; }
    public IReadOnlyList<string> ValidationErrors { get; }
    public string? ErrorMessage { get; }

    private RegisterUserCommandResult(
        bool isSuccess,
        UserId? userId = null,
        IReadOnlyList<string>? validationErrors = null,
        string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        UserId = userId;
        ValidationErrors = validationErrors ?? Array.Empty<string>();
        ErrorMessage = errorMessage;
    }

    public static RegisterUserCommandResult Success(UserId userId) =>
        new(true, userId);

    public static RegisterUserCommandResult ValidationFailure(IReadOnlyList<string> validationErrors) =>
        new(false, validationErrors: validationErrors);

    public static RegisterUserCommandResult Failure(string errorMessage) =>
        new(false, errorMessage: errorMessage);

    public static RegisterUserCommandResult EmailAlreadyExists(string email) =>
        new(false, errorMessage: $"A user with email '{email}' already exists");

    public static RegisterUserCommandResult UsernameAlreadyExists(string username) =>
        new(false, errorMessage: $"A user with username '{username}' already exists");
}
