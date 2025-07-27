namespace WeaponApi.Domain.Weapon;

public sealed record WeaponId
{
    public Guid Value { get; }

    private WeaponId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("WeaponId cannot be empty", nameof(value));

        Value = value;
    }

    public static WeaponId Create() => new(Guid.NewGuid());

    public static WeaponId Create(Guid value) => new(value);

    public static implicit operator Guid(WeaponId weaponId) => weaponId.Value;

    public override string ToString() => Value.ToString();
}
