using MediatR;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Command to repair a weapon by reducing its damage and increasing hit points.
/// </summary>
public sealed record RepairWeaponCommand(
    WeaponId WeaponId,
    int RepairAmount
) : IRequest<RepairWeaponCommandResult>;

/// <summary>
/// Result type for RepairWeaponCommand execution.
/// </summary>
public sealed class RepairWeaponCommandResult
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }

    private RepairWeaponCommandResult(bool isSuccess, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static RepairWeaponCommandResult Success() =>
        new(true);

    public static RepairWeaponCommandResult NotFound(WeaponId weaponId) =>
        new(false, $"Weapon with ID '{weaponId}' was not found");

    public static RepairWeaponCommandResult Failure(string errorMessage) =>
        new(false, errorMessage);
}
