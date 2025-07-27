using MediatR;
using WeaponApi.Application.Weapon;
using WeaponApi.Application.Weapon.Commands;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Handler for CreateRandomWeaponCommand.
/// Generates a random weapon with MaxHitPoints of 100 and appropriate value based on type.
/// </summary>
public sealed class CreateRandomWeaponCommandHandler : IRequestHandler<CreateRandomWeaponCommand, CreateRandomWeaponCommandResult>
{
    private readonly IWeaponRepository weaponRepository;
    private readonly Random random;

    public CreateRandomWeaponCommandHandler(IWeaponRepository weaponRepository)
    {
        this.weaponRepository = weaponRepository ?? throw new ArgumentNullException(nameof(weaponRepository));
        this.random = new Random();
    }

    public async Task<CreateRandomWeaponCommandResult> Handle(CreateRandomWeaponCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Generate random weapon properties
            var weaponType = GenerateRandomWeaponType();
            var weaponName = WeaponName.Generate(weaponType, GenerateRandomAdjective());
            var description = GenerateRandomDescription(weaponType);
            const int maxHitPoints = 100;
            var damage = GenerateRandomDamage();
            var isRepairable = random.Next(0, 10) > 2; // 70% chance of being repairable
            var value = CalculateWeaponValue(weaponType, maxHitPoints);

            // Create the weapon
            var weapon = Domain.Weapon.Weapon.Create(
                weaponName,
                description,
                maxHitPoints,
                damage,
                isRepairable,
                value);

            // Save to repository
            await weaponRepository.AddAsync(weapon);

            return CreateRandomWeaponCommandResult.Success(weapon.Id);
        }
        catch (Exception ex)
        {
            return CreateRandomWeaponCommandResult.Failure($"Failed to create random weapon: {ex.Message}");
        }
    }

    private WeaponType GenerateRandomWeaponType()
    {
        var weaponTypes = Enum.GetValues<WeaponType.WeaponTypeEnum>();
        var randomType = weaponTypes[random.Next(weaponTypes.Length)];
        return WeaponType.Create(randomType);
    }

    private string GenerateRandomAdjective()
    {
        var adjectives = new[]
        {
            "Mighty", "Swift", "Ancient", "Gleaming", "Rusty", "Sharp", "Dull",
            "Enchanted", "Cursed", "Legendary", "Common", "Rare", "Epic", "Divine",
            "Broken", "Pristine", "Weathered", "Polished", "Ornate", "Simple"
        };
        return adjectives[random.Next(adjectives.Length)];
    }

    private string GenerateRandomDescription(WeaponType weaponType)
    {
        var descriptions = new[]
        {
            $"A {(weaponType.IsMagicWeapon() ? "magical" : "sturdy")} {weaponType.Value.ToString().ToLower()} forged by skilled artisans.",
            $"This {weaponType.Value.ToString().ToLower()} has seen many battles and bears the scars of combat.",
            $"A {(weaponType.IsMagicWeapon() ? "mystical" : "reliable")} {weaponType.Value.ToString().ToLower()} with excellent balance.",
            $"An {(weaponType.IsMagicWeapon() ? "enchanted" : "ordinary")} {weaponType.Value.ToString().ToLower()} crafted for warriors.",
            $"This {weaponType.Value.ToString().ToLower()} radiates {(weaponType.IsMagicWeapon() ? "magical energy" : "craftsmanship")}."
        };
        return descriptions[random.Next(descriptions.Length)];
    }

    private int GenerateRandomDamage()
    {
        return random.Next(5, 21); // Damage between 5-20
    }

    private decimal CalculateWeaponValue(WeaponType weaponType, int maxHitPoints)
    {
        var baseValue = maxHitPoints * 2m; // Base value is 2x max hit points

        if (weaponType.IsMagicWeapon())
        {
            var magicalBonus = random.Next(2, 6); // Magical bonus between 2-5
            var maxValue = maxHitPoints * magicalBonus;
            return Math.Min(baseValue * random.Next(2, 4), maxValue); // 2-3x base value, capped by max
        }

        return baseValue * (decimal)(0.5 + random.NextDouble()); // 0.5-1.5x base value for non-magical
    }
}
