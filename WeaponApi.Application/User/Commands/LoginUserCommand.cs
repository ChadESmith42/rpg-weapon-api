using MediatR;
using WeaponApi.Domain.User;

namespace WeaponApi.Application.User.Commands;

/// <summary>
/// Command to authenticate a user login attempt.
/// Follows MediatR CQRS pattern for user authentication handling.
/// </summary>
public sealed record LoginUserCommand(
    string EmailOrUsername,
    string Password
) : IRequest<LoginUserCommandResult>;

/// <summary>
/// Result type for LoginUserCommand execution.
/// Provides structured response with authentication data or error information.
/// </summary>
public sealed class LoginUserCommandResult
{
    public bool IsSuccess { get; }
    public Domain.User.User? User { get; }
    public IReadOnlyList<Role> Roles { get; }
    public string? ErrorMessage { get; }

    private LoginUserCommandResult(
        bool isSuccess, 
        Domain.User.User? user = null, 
        IReadOnlyList<Role>? roles = null, 
        string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        User = user;
        Roles = roles ?? Array.Empty<Role>();
        ErrorMessage = errorMessage;
    }

    public static LoginUserCommandResult Success(Domain.User.User user, IReadOnlyList<Role> roles) => 
        new(true, user, roles);

    public static LoginUserCommandResult InvalidCredentials() => 
        new(false, errorMessage: "Invalid email/username or password");

    public static LoginUserCommandResult UserNotFound() => 
        new(false, errorMessage: "User not found");

    public static LoginUserCommandResult Failure(string errorMessage) => 
        new(false, errorMessage: errorMessage);

    public static LoginUserCommandResult ValidationFailure(string validationError) => 
        new(false, errorMessage: validationError);
}
