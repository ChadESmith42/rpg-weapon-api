namespace WeaponApi.Domain.Weapon;

/// <summary>
/// Value object representing a weapon's name.
/// Generates names in the format "[WeaponType] of [descriptor]".
/// </summary>
public sealed record WeaponName
{
    private static readonly string[] Descriptors =
    {
        "Flames",
        "Ice",
        "Lightning",
        "Shadows",
        "Light",
        "Darkness",
        "Thunder",
        "Frost",
        "Poison",
        "Healing",
        "Power",
        "Strength",
        "Wisdom",
        "Courage",
        "Honor",
        "Glory",
        "Victory",
        "Destruction",
        "Creation",
        "Magic",
        "Loathing",
        "Fury",
        "Wrath",
        "Storm",
        "Wind",
        "Earth",
        "Water",
        "Fire",
        "Nature",
        "Spirit",
        "Soul",
        "Chaos",
        "Order",
        "Time",
        "Space",
        "Void",
        "Hate",
        "Love",
        "Life",
        "Death",
        "Steel",
        "Silver",
        "Gold",
        "Diamond",
        "Crystal",
        "Dragon's Breath",
        "Phoenix Rising",
        "Storm's Fury",
        "Ancient Power",
        "Eternal Flame",
        "Frozen Death",
        "Swift Justice",
        "Divine Wrath",
        "Mystic Force",
        "Endless Night",
        "Blazing Sun",
        "Raging Storm",
        "Silent Death",
        "Piercing Wind",
        "Crushing Stone",
        "Flowing Water",
        "Burning Earth",
        "Shining Stars",
        "Falling Meteors",
        "Rising Moon",
        "Dancing Leaves",
        "Whispering Winds",
        "Roaring Flames",
        "Crashing Waves",
        "Trembling Earth",
        "Pudding",
        "Dave"
    };

    public string Value { get; }

    private WeaponName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Weapon name cannot be empty", nameof(value));

        Value = value;
    }

    public static WeaponName Create(string customName) => new(customName);

    public static WeaponName Generate(WeaponType weaponType)
    {
        if (weaponType == null)
            throw new ArgumentNullException(nameof(weaponType));

        var random = new Random();
        var descriptor = Descriptors[random.Next(Descriptors.Length)];
        var generatedName = $"{weaponType} of {descriptor}";

        return new WeaponName(generatedName);
    }

    public static WeaponName Generate(WeaponType weaponType, string defaultDescriptor)
    {
        if (weaponType == null)
            throw new ArgumentNullException(nameof(weaponType));
        if (string.IsNullOrEmpty(defaultDescriptor))
            throw new ArgumentException("Default descriptor cannot be null or empty", nameof(defaultDescriptor));
        if (!Descriptors.Contains(defaultDescriptor, StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Default descriptor must be one of the predefined descriptors", nameof(defaultDescriptor));

        var generatedName = $"{weaponType} of {defaultDescriptor}";

        return new WeaponName(generatedName);
    }

    public static WeaponName Generate(WeaponType weaponType, int seed)
    {
        if (weaponType == null)
            throw new ArgumentNullException(nameof(weaponType));

        var random = new Random(seed);
        var descriptor = Descriptors[random.Next(Descriptors.Length)];
        var generatedName = $"{weaponType} of {descriptor}";

        return new WeaponName(generatedName);
    }

    public bool ContainsDescriptor(string descriptor)
    {
        if (string.IsNullOrWhiteSpace(descriptor))
            return false;

        return Value.Contains($"of {descriptor}", StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString() => Value;
}
