namespace WeaponApi.Domain.Weapon;

public sealed class WeaponType
{
  public WeaponTypeEnum Value { get; }

  private WeaponType(WeaponTypeEnum value)
  {
    if (!Enum.IsDefined(typeof(WeaponTypeEnum), value))
    {
      throw new ArgumentException("Invalid weapon type value", nameof(value));
    }
    Value = value;
  }

  public static WeaponType Create(WeaponTypeEnum value) => new WeaponType(value);

  public static WeaponType Sword => Create(WeaponTypeEnum.Sword);
  public static WeaponType Axe => Create(WeaponTypeEnum.Axe);
  public static WeaponType Bow => Create(WeaponTypeEnum.Bow);
  public static WeaponType Crossbow => Create(WeaponTypeEnum.Crossbow);
  public static WeaponType Staff => Create(WeaponTypeEnum.Staff);
  public static WeaponType Dagger => Create(WeaponTypeEnum.Dagger);
  public static WeaponType Wand => Create(WeaponTypeEnum.Wand);
  public static WeaponType Spear => Create(WeaponTypeEnum.Spear);
  public static WeaponType Mace => Create(WeaponTypeEnum.Mace);
  public static WeaponType Hammer => Create(WeaponTypeEnum.Hammer);
  public static WeaponType Shield => Create(WeaponTypeEnum.Shield);
  public static WeaponType Spoon => Create(WeaponTypeEnum.Spoon);
  public static WeaponType Sponge => Create(WeaponTypeEnum.Sponge);
  public static WeaponType Stick => Create(WeaponTypeEnum.Stick);
  public static WeaponType Sling => Create(WeaponTypeEnum.Sling);
  public static WeaponType Knife => Create(WeaponTypeEnum.Knife);
  public static WeaponType Flail => Create(WeaponTypeEnum.Flail);
  public static WeaponType Scythe => Create(WeaponTypeEnum.Scythe);
  public static WeaponType Halberd => Create(WeaponTypeEnum.Halberd);
  public static WeaponType Trident => Create(WeaponTypeEnum.Trident);
  public static WeaponType Whip => Create(WeaponTypeEnum.Whip);
  public static WeaponType Polearm => Create(WeaponTypeEnum.Polearm);

  public bool IsMeleeWeapon() => Value is WeaponTypeEnum.Sword or
                                  WeaponTypeEnum.Axe or
                                  WeaponTypeEnum.Dagger or
                                  WeaponTypeEnum.Mace or
                                  WeaponTypeEnum.Hammer or
                                  WeaponTypeEnum.Flail or
                                  WeaponTypeEnum.Scythe or
                                  WeaponTypeEnum.Halberd or
                                  WeaponTypeEnum.Trident or
                                  WeaponTypeEnum.Whip or
                                  WeaponTypeEnum.Polearm or
                                  WeaponTypeEnum.Spoon or
                                  WeaponTypeEnum.Sponge or
                                  WeaponTypeEnum.Stick or
                                  WeaponTypeEnum.Knife or
                                  WeaponTypeEnum.Spear or
                                  WeaponTypeEnum.Staff or
                                  WeaponTypeEnum.Shield;

  public bool IsRangedWeapon() => Value is WeaponTypeEnum.Bow or
                                      WeaponTypeEnum.Crossbow or
                                      WeaponTypeEnum.Sling or
                                      WeaponTypeEnum.Staff or
                                      WeaponTypeEnum.Spear or
                                      WeaponTypeEnum.Trident or
                                      WeaponTypeEnum.Hammer or
                                      WeaponTypeEnum.Whip or
                                      WeaponTypeEnum.Wand;

  public bool IsMagicWeapon() => Value is WeaponTypeEnum.Staff or
                                  WeaponTypeEnum.Wand or
                                  WeaponTypeEnum.Scythe or
                                  WeaponTypeEnum.Trident or
                                  WeaponTypeEnum.Spoon or
                                  WeaponTypeEnum.Stick or
                                  WeaponTypeEnum.Sponge;

  public override string ToString() => Value.ToString();

  public enum WeaponTypeEnum
  {
    Sword,
    Axe,
    Bow,
    Crossbow,
    Staff,
    Dagger,
    Wand,
    Spear,
    Mace,
    Hammer,
    Shield,
    Spoon,
    Sponge,
    Stick,
    Sling,
    Knife,
    Flail,
    Scythe,
    Halberd,
    Trident,
    Whip,
    Polearm,
  }
}
