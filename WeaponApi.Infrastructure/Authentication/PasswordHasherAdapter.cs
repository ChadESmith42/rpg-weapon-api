using WeaponApi.Application.Authentication;
using WeaponApi.Domain.User;

namespace WeaponApi.Infrastructure.Authentication;

/// <summary>
/// Adapter that implements the domain IPasswordHasher interface
/// by delegating to the application layer IPasswordHashService.
/// </summary>
public sealed class PasswordHasherAdapter : IPasswordHasher
{
    private readonly IPasswordHashService _passwordHashService;

    public PasswordHasherAdapter(IPasswordHashService passwordHashService)
    {
        _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
    }

    public string HashPassword(string password)
    {
        return _passwordHashService.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return _passwordHashService.VerifyPassword(password, hashedPassword);
    }
}
