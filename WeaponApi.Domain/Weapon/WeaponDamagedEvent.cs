using WeaponApi.Domain.Common;

namespace WeaponApi.Domain.Weapon;

/// <summary>
/// Domain event raised when a weapon takes damage.
/// </summary>
public sealed record WeaponDamagedEvent : DomainEvent
{
    public WeaponId WeaponId { get; }
    public int DamageAmount { get; }
    public int NewHitPoints { get; }
    public int NewDamageLevel { get; }
    public decimal NewValue { get; }

    public WeaponDamagedEvent(WeaponId weaponId, int damageAmount, int newHitPoints, int newDamageLevel, decimal newValue)
    {
        WeaponId = weaponId;
        DamageAmount = damageAmount;
        NewHitPoints = newHitPoints;
        NewDamageLevel = newDamageLevel;
        NewValue = newValue;
    }
}
