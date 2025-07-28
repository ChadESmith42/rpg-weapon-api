using MediatR;
using WeaponApi.Application.Authentication;
using WeaponApi.Application.User.Commands;
using WeaponApi.Domain.User;

namespace WeaponApi.Application.User.Handlers;

/// <summary>
/// Handles user registration command processing.
/// Validates user data, checks for duplicates, and creates new user accounts.
/// </summary>
public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<RegisterUserCommandResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate password strength first
            var passwordValidation = _passwordHashService.ValidatePasswordStrength(request.Password);
            if (!passwordValidation.IsValid)
            {
                return RegisterUserCommandResult.ValidationFailure(passwordValidation.ErrorMessages.ToArray());
            }

            // Create email domain object
            var email = Email.Create(request.Email);

            // Check if username already exists
            var existingUserByUsername = await _userRepository.FindByUsernameAsync(request.Username);
            if (existingUserByUsername != null)
            {
                return RegisterUserCommandResult.ValidationFailure(new[] { "Username already exists" });
            }

            // Check if email already exists - return generic validation error for security
            var existingUserByEmail = await _userRepository.FindByEmailAsync(email);
            if (existingUserByEmail != null)
            {
                return RegisterUserCommandResult.ValidationFailure(new[] { "Email already exists" });
            }

            // Create the user using domain factory method
            var user = Domain.User.User.Register(
                username: request.Username,
                name: request.Name,
                email: email,
                password: request.Password,
                dateOfBirth: request.DateOfBirth,
                passwordHasher: _passwordHasher
            );

            // Save the user
            await _userRepository.AddAsync(user);

            return RegisterUserCommandResult.Success(user.Id);
        }
        catch (Exception ex)
        {
            return RegisterUserCommandResult.Failure($"Registration failed: {ex.Message}");
        }
    }
}
