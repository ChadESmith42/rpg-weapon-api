using MediatR;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Command to create a new weapon in the system.
/// Follows MediatR CQRS pattern for command handling.
/// </summary>
public sealed record CreateWeaponCommand(
    string Name,
    WeaponType.WeaponTypeEnum Type,
    string Description,
    int HitPoints,
    int Damage,
    bool IsRepairable,
    decimal Value
) : IRequest<CreateWeaponCommandResult>;

/// <summary>
/// Result type for CreateWeaponCommand execution.
/// Provides structured response with success/failure information.
/// </summary>
public sealed class CreateWeaponCommandResult
{
    public bool IsSuccess { get; }
    public WeaponId? WeaponId { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyList<string> ValidationErrors { get; }

    private CreateWeaponCommandResult(bool isSuccess, WeaponId? weaponId = null, string? errorMessage = null, IReadOnlyList<string>? validationErrors = null)
    {
        IsSuccess = isSuccess;
        WeaponId = weaponId;
        ErrorMessage = errorMessage;
        ValidationErrors = validationErrors ?? Array.Empty<string>();
    }

    public static CreateWeaponCommandResult Success(WeaponId weaponId) =>
        new(true, weaponId);

    public static CreateWeaponCommandResult Failure(string errorMessage) =>
        new(false, errorMessage: errorMessage);

    public static CreateWeaponCommandResult ValidationFailure(params string[] validationErrors) =>
        new(false, validationErrors: validationErrors.ToList().AsReadOnly());

    public static CreateWeaponCommandResult ValidationFailure(IEnumerable<string> validationErrors) =>
        new(false, validationErrors: validationErrors.ToList().AsReadOnly());
}
