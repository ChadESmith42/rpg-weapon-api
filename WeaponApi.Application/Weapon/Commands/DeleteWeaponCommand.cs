using MediatR;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Command to delete a weapon from the system.
/// Follows MediatR CQRS pattern for command handling.
/// </summary>
public sealed record DeleteWeaponCommand(WeaponId WeaponId) : IRequest<DeleteWeaponCommandResult>;

/// <summary>
/// Result type for DeleteWeaponCommand execution.
/// Provides structured response indicating success or failure.
/// </summary>
public sealed class DeleteWeaponCommandResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyList<string> ValidationErrors { get; }

    private DeleteWeaponCommandResult(bool isSuccess, string? errorMessage = null, IReadOnlyList<string>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors ?? Array.Empty<string>();
    }

    public static DeleteWeaponCommandResult Success() =>
        new(true);

    public static DeleteWeaponCommandResult NotFound(WeaponId weaponId) =>
        new(false, $"Weapon with ID '{weaponId}' was not found");

    public static DeleteWeaponCommandResult Failure(string errorMessage) =>
        new(false, errorMessage);
}
