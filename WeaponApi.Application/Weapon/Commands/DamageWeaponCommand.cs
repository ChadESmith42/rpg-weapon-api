using MediatR;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Command to damage a weapon by reducing its hit points.
/// </summary>
public sealed record DamageWeaponCommand(
    WeaponId WeaponId,
    int DamageAmount
) : IRequest<DamageWeaponCommandResult>;

/// <summary>
/// Result type for DamageWeaponCommand execution.
/// </summary>
public sealed class DamageWeaponCommandResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    private DamageWeaponCommandResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static DamageWeaponCommandResult Success() =>
        new(true);

    public static DamageWeaponCommandResult NotFound(WeaponId weaponId) =>
        new(false, $"Weapon with ID '{weaponId}' was not found");

    public static DamageWeaponCommandResult Failure(string errorMessage) =>
        new(false, errorMessage);
}
