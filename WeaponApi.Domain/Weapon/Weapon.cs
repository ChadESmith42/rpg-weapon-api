namespace WeaponApi.Domain.Weapon;

public class Weapon
{
  public WeaponId Id { get; }
  public WeaponType Type { get; private set; } = WeaponType.Spoon;
  public WeaponName Name { get; private set; } = WeaponName.Generate(WeaponType.Spoon, "Pudding");
  public string Description { get; private set; } = string.Empty;
  public int HitPoints { get; private set; }
  public int MaxHitPoints { get; private set; }
  public int Damage { get; private set; }
  public bool IsRepairable { get; private set; }
  public decimal Value { get; private set; }

  // Private parameterless constructor for Entity Framework
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
  private Weapon()
  {
  }
#pragma warning restore CS8618

  private Weapon(WeaponId id, WeaponName name, string description, int hitPoints, int damage, bool isRepairable, decimal value)
  {
    Id = id;
    Name = name;
    Description = description;
    HitPoints = hitPoints;
    MaxHitPoints = hitPoints; // Set MaxHitPoints to the initial hitPoints value
    Damage = damage;
    IsRepairable = isRepairable;
    Value = value;
  }

  public static Weapon Create(WeaponName name, string description, int hitPoints, int damage, bool isRepairable, decimal value)
  {
    var id = WeaponId.Create();
    return new Weapon(id, name, description, hitPoints, damage, isRepairable, value);
  }

  public void DamageWeapon(int damageAmount)
  {
    this.Damage += damageAmount;
    this.HitPoints = Math.Max(this.MaxHitPoints - damageAmount, 0);

    decimal damageRatio = (decimal)this.Damage / this.MaxHitPoints;
    this.Value += 1 - (damageRatio * 0.5m);
  }

  public void RepairWeapon(int repairAmount)
  {
    if (!IsRepairable)
    {
      throw new InvalidOperationException("This weapon cannot be repaired.");
    }

    int actualRepair = Math.Min(repairAmount, this.Damage);
    this.Damage -= actualRepair;
    this.HitPoints = Math.Min(this.MaxHitPoints, this.HitPoints + actualRepair);

    decimal repairRatio = (decimal)actualRepair / this.MaxHitPoints;

    this.Value = this.Type.IsMagicWeapon() ? this.Value + (repairRatio * 0.5m) : this.Value + (repairRatio * 0.3m);
  }

  public void ValidateWeapon()
  {
    if (this.HitPoints <= 0)
    {
      throw new InvalidOperationException("This weapon is broken and cannot be used.");
    }

    if (this.Damage < 0)
    {
      throw new InvalidOperationException("Damage cannot be negative.");
    }
    if (this.MaxHitPoints <= 0)
    {
      throw new InvalidOperationException("Max hit points must be greater than zero.");
    }
    if (this.Value < 0)
    {
      throw new InvalidOperationException("Value cannot be negative.");
    }
  }

  public RepairEstimate GetRepairEstimate(int repairAmount)
  {
    if (!IsRepairable)
    {
      throw new InvalidOperationException("This weapon cannot be repaired.");
    }

    int actualRepair = Math.Min(repairAmount, this.Damage);
    int gainedHitPoints = Math.Min(this.MaxHitPoints, this.HitPoints + actualRepair);

    decimal repairRatio = (decimal)actualRepair / this.MaxHitPoints;
    decimal valueMultiplier = this.Type.IsMagicWeapon() ? 0.5m : 0.3m;
    decimal gainedValue = this.Value * (repairRatio * valueMultiplier);

    int repairCostPerPoint = this.Type.IsMagicWeapon() ? 3 : 2;
    int repairCost = actualRepair * repairCostPerPoint;

    return new RepairEstimate(
      repairCost,
      gainedHitPoints,
      Math.Round(gainedValue, 2)
    );
  }
}
