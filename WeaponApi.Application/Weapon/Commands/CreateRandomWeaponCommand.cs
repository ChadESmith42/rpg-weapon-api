using MediatR;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Command to create a random weapon with predefined characteristics.
/// Generates a weapon with MaxHitPoints of 100 and value based on weapon type.
/// </summary>
public sealed record CreateRandomWeaponCommand() : IRequest<CreateRandomWeaponCommandResult>;

/// <summary>
/// Result type for CreateRandomWeaponCommand execution.
/// Provides structured response with success/failure information.
/// </summary>
public sealed class CreateRandomWeaponCommandResult
{
    public bool IsSuccess { get; }
    public WeaponId? WeaponId { get; }
    public string? ErrorMessage { get; }

    private CreateRandomWeaponCommandResult(bool isSuccess, WeaponId? weaponId = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        WeaponId = weaponId;
        ErrorMessage = errorMessage;
    }

    public static CreateRandomWeaponCommandResult Success(WeaponId weaponId) =>
        new(true, weaponId);

    public static CreateRandomWeaponCommandResult Failure(string errorMessage) =>
        new(false, errorMessage: errorMessage);
}
