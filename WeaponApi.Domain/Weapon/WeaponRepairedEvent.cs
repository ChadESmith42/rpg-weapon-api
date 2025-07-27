using WeaponApi.Domain.Common;

namespace WeaponApi.Domain.Weapon;

/// <summary>
/// Domain event raised when a weapon is repaired.
/// </summary>
public sealed record WeaponRepairedEvent : DomainEvent
{
    public WeaponId WeaponId { get; }
    public int RepairAmount { get; }
    public int NewHitPoints { get; }
    public int NewDamageLevel { get; }
    public decimal NewValue { get; }
    public decimal RepairCost { get; }

    public WeaponRepairedEvent(WeaponId weaponId, int repairAmount, int newHitPoints, int newDamageLevel, decimal newValue, decimal repairCost)
    {
        WeaponId = weaponId;
        RepairAmount = repairAmount;
        NewHitPoints = newHitPoints;
        NewDamageLevel = newDamageLevel;
        NewValue = newValue;
        RepairCost = repairCost;
    }
}
