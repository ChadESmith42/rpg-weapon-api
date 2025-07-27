using WeaponApi.Domain.Common;

namespace WeaponApi.Domain.Weapon;

/// <summary>
/// Domain event raised when a weapon is created.
/// </summary>
public sealed record WeaponCreatedEvent : DomainEvent
{
    public WeaponId WeaponId { get; }
    public WeaponName WeaponName { get; }
    public string Description { get; }
    public int InitialHitPoints { get; }
    public int Damage { get; }
    public bool IsRepairable { get; }
    public decimal InitialValue { get; }

    public WeaponCreatedEvent(WeaponId weaponId, WeaponName weaponName, string description, int initialHitPoints, int damage, bool isRepairable, decimal initialValue)
    {
        WeaponId = weaponId;
        WeaponName = weaponName;
        Description = description;
        InitialHitPoints = initialHitPoints;
        Damage = damage;
        IsRepairable = isRepairable;
        InitialValue = initialValue;
    }
}
