using MediatR;
using WeaponApi.Application.User.Commands;
using WeaponApi.Domain.User;

namespace WeaponApi.Application.User.Handlers;

/// <summary>
/// Handles user login command processing.
/// Validates credentials and authenticates users.
/// </summary>
public sealed class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<LoginUserCommandResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Try to find user by email first, then by username
            Domain.User.User? user = null;

            // Try as email first
            try
            {
                var email = Email.Create(request.EmailOrUsername);
                user = await _userRepository.FindByEmailAsync(email);
            }
            catch (ArgumentException)
            {
                // Not a valid email format, ignore and try as username
            }

            // If not found by email, try by username
            user ??= await _userRepository.FindByUsernameAsync(request.EmailOrUsername);

            if (user == null)
            {
                return LoginUserCommandResult.UserNotFound();
            }

            // Verify password using domain service
            var isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.Security.PasswordHash);
            if (!isPasswordValid)
            {
                return LoginUserCommandResult.InvalidCredentials();
            }

            // Record login
            user.RecordLogin();
            await _userRepository.UpdateAsync(user);

            return LoginUserCommandResult.Success(user, user.Roles);
        }
        catch (Exception ex)
        {
            return LoginUserCommandResult.Failure($"Login failed: {ex.Message}");
        }
    }
}
