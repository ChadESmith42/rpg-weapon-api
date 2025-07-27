using System.Net.NetworkInformation;

namespace WeaponApi.Domain.Weapon;

public class WeaponId
{
  public Guid Value { get; }

  public WeaponId(Guid value)
  {
    if (value == Guid.Empty)
    {
      throw new ArgumentException("WeaponId cannot be an empty GUID.", nameof(value));
    }
    Value = value;
  }

  public static WeaponId Create() => new(Guid.NewGuid());

  public static WeaponId Create(Guid value) => new(value);

  public static implicit operator Guid(WeaponId weaponId) => weaponId.Value;

  public override string ToString() => Value.ToString();
}
