using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.User.DTOs;

/// <summary>
/// Data transfer object for weapon information in user contexts.
/// Represents a simplified view of weapon data for API responses.
/// DTOs are exempt from certain Object Calisthenics rules per architectural guidelines.
/// </summary>
public sealed record WeaponDto(
    string Id,
    string Name,
    string Type,
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
    /// Creates WeaponDto from a domain Weapon entity.
    /// Converts domain value objects to simple types for API consumption.
    /// </summary>
    /// <param name="weapon">The domain weapon entity</param>
    /// <returns>WeaponDto for API response</returns>
    public static WeaponDto FromDomain(Domain.Weapon.Weapon weapon)
    {
        if (weapon == null)
            throw new ArgumentNullException(nameof(weapon));

        return new WeaponDto(
            weapon.Id.Value.ToString(),
            weapon.Name.Value,
            weapon.Type.ToString(),
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
