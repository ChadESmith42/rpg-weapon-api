using Microsoft.EntityFrameworkCore;
using WeaponApi.Application.Weapon;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Weapon aggregate root operations.
/// Provides business-oriented data access methods using Entity Framework Core.
/// </summary>
public sealed class WeaponRepository : IWeaponRepository
{
    private readonly WeaponApiDbContext context;

    public WeaponRepository(WeaponApiDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Weapon?> FindByIdAsync(WeaponId id, CancellationToken cancellationToken = default)
    {
        return await context.Weapons
            .FirstOrDefaultAsync(weapon => weapon.Id == id, cancellationToken);
    }

    public async Task AddAsync(Weapon aggregate, CancellationToken cancellationToken = default)
    {
        await context.Weapons.AddAsync(aggregate, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Weapon aggregate, CancellationToken cancellationToken = default)
    {
        context.Weapons.Update(aggregate);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Weapon aggregate, CancellationToken cancellationToken = default)
    {
        context.Weapons.Remove(aggregate);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(WeaponId id, CancellationToken cancellationToken = default)
    {
        return await context.Weapons
            .AnyAsync(weapon => weapon.Id == id, cancellationToken);
    }

    public async Task<Weapon?> FindByNameAsync(WeaponName name, CancellationToken cancellationToken = default)
    {
        return await context.Weapons
            .FirstOrDefaultAsync(weapon => weapon.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Weapon>> FindByTypeAsync(WeaponType type, CancellationToken cancellationToken = default)
    {
        var weapons = await context.Weapons
            .Where(weapon => weapon.Type == type)
            .ToListAsync(cancellationToken);

        return weapons.AsReadOnly();
    }

    public async Task<IReadOnlyList<Weapon>> FindDamagedWeaponsAsync(CancellationToken cancellationToken = default)
    {
        var weapons = await context.Weapons
            .Where(weapon => weapon.Damage > 0)
            .ToListAsync(cancellationToken);

        return weapons.AsReadOnly();
    }

    public async Task<IReadOnlyList<Weapon>> FindByValueRangeAsync(decimal minValue, decimal maxValue, CancellationToken cancellationToken = default)
    {
        var weapons = await context.Weapons
            .Where(weapon => weapon.Value >= minValue && weapon.Value <= maxValue)
            .ToListAsync(cancellationToken);

        return weapons.AsReadOnly();
    }

    public async Task<IReadOnlyList<Weapon>> FindRepairableWeaponsAsync(CancellationToken cancellationToken = default)
    {
        var weapons = await context.Weapons
            .Where(weapon => weapon.IsRepairable)
            .ToListAsync(cancellationToken);

        return weapons.AsReadOnly();
    }

    public async Task<IReadOnlyList<Weapon>> FindMeleeWeaponsAsync(CancellationToken cancellationToken = default)
    {
        var meleeTypes = GetMeleeWeaponTypes();

        var weapons = await context.Weapons
            .Where(weapon => meleeTypes.Contains(weapon.Type.Value))
            .ToListAsync(cancellationToken);

        return weapons.AsReadOnly();
    }

    public async Task<IReadOnlyList<Weapon>> FindRangedWeaponsAsync(CancellationToken cancellationToken = default)
    {
        var rangedTypes = GetRangedWeaponTypes();

        var weapons = await context.Weapons
            .Where(weapon => rangedTypes.Contains(weapon.Type.Value))
            .ToListAsync(cancellationToken);

        return weapons.AsReadOnly();
    }

    public async Task<IReadOnlyList<Weapon>> FindHighValueWeaponsAsync(decimal threshold, CancellationToken cancellationToken = default)
    {
        var weapons = await context.Weapons
            .Where(weapon => weapon.Value >= threshold)
            .OrderByDescending(weapon => weapon.Value)
            .ToListAsync(cancellationToken);

        return weapons.AsReadOnly();
    }

    public async Task<IReadOnlyList<Weapon>> FindLowDurabilityWeaponsAsync(int maxHitPoints, CancellationToken cancellationToken = default)
    {
        var weapons = await context.Weapons
            .Where(weapon => weapon.HitPoints <= maxHitPoints)
            .ToListAsync(cancellationToken);

        return weapons.AsReadOnly();
    }

    private static WeaponType.WeaponTypeEnum[] GetMeleeWeaponTypes()
    {
        return new[]
        {
            WeaponType.WeaponTypeEnum.Sword,
            WeaponType.WeaponTypeEnum.Axe,
            WeaponType.WeaponTypeEnum.Dagger,
            WeaponType.WeaponTypeEnum.Mace,
            WeaponType.WeaponTypeEnum.Hammer,
            WeaponType.WeaponTypeEnum.Flail,
            WeaponType.WeaponTypeEnum.Scythe,
            WeaponType.WeaponTypeEnum.Halberd,
            WeaponType.WeaponTypeEnum.Trident,
            WeaponType.WeaponTypeEnum.Whip,
            WeaponType.WeaponTypeEnum.Spear,
            WeaponType.WeaponTypeEnum.Polearm,
            WeaponType.WeaponTypeEnum.Spoon,
            WeaponType.WeaponTypeEnum.Knife
        };
    }

    private static WeaponType.WeaponTypeEnum[] GetRangedWeaponTypes()
    {
        return new[]
        {
            WeaponType.WeaponTypeEnum.Bow,
            WeaponType.WeaponTypeEnum.Crossbow,
            WeaponType.WeaponTypeEnum.Sling
        };
    }
}
