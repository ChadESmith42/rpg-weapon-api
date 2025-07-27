using MediatR;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Queries;

/// <summary>
/// Query to retrieve a weapon by its unique identifier.
/// Follows MediatR CQRS pattern for query handling.
/// </summary>
public sealed record GetWeaponQuery(WeaponId WeaponId) : IRequest<GetWeaponQueryResult>;

/// <summary>
/// Result type for GetWeaponQuery execution.
/// Provides structured response with weapon data or error information.
/// </summary>
public sealed class GetWeaponQueryResult
{
    public bool IsSuccess { get; }
    public WeaponData? Weapon { get; }
    public string? ErrorMessage { get; }

    private GetWeaponQueryResult(bool isSuccess, WeaponData? weapon = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        Weapon = weapon;
        ErrorMessage = errorMessage;
    }

    public static GetWeaponQueryResult Success(WeaponData weapon) => 
        new(true, weapon);

    public static GetWeaponQueryResult NotFound(WeaponId weaponId) => 
        new(false, errorMessage: $"Weapon with ID '{weaponId}' was not found");

    public static GetWeaponQueryResult Failure(string errorMessage) => 
        new(false, errorMessage: errorMessage);
}

/// <summary>
/// Data transfer object for weapon information in query responses.
/// Encapsulates weapon data for read operations.
/// </summary>
public sealed record WeaponData(
    WeaponId Id,
    WeaponName Name,
    WeaponType Type,
    string Description,
    int HitPoints,
    int MaxHitPoints,
    int Damage,
    bool IsRepairable,
    decimal Value,
    bool IsDamaged
)
{
    /// <summary>
    /// Creates WeaponData from a domain Weapon entity.
    /// </summary>
    /// <param name="weapon">The domain weapon entity</param>
    /// <returns>WeaponData for query response</returns>
    public static WeaponData FromDomain(Domain.Weapon.Weapon weapon)
    {
        if (weapon == null)
            throw new ArgumentNullException(nameof(weapon));

        return new WeaponData(
            weapon.Id,
            weapon.Name,
            weapon.Type,
            weapon.Description,
            weapon.HitPoints,
            weapon.MaxHitPoints,
            weapon.Damage,
            weapon.IsRepairable,
            weapon.Value,
            weapon.HitPoints < weapon.MaxHitPoints
        );
    }
}
