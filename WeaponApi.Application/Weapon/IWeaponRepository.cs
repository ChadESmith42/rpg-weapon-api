using WeaponApi.Application.Common;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon;

/// <summary>
/// Repository interface for Weapon aggregate operations.
/// Provides business-oriented methods for weapon persistence and retrieval.
/// </summary>
public interface IWeaponRepository : IRepository<Domain.Weapon.Weapon, WeaponId>
{
    /// <summary>
    /// Finds a weapon by its name.
    /// </summary>
    /// <param name="name">The weapon name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The weapon if found, null otherwise</returns>
    Task<Domain.Weapon.Weapon?> FindByNameAsync(WeaponName name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all weapons of a specific type.
    /// </summary>
    /// <param name="weaponType">The weapon type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of weapons of the specified type</returns>
    Task<IReadOnlyList<Domain.Weapon.Weapon>> FindByTypeAsync(WeaponType weaponType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves weapons that require repair (hit points below maximum).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of damaged weapons</returns>
    Task<IReadOnlyList<Domain.Weapon.Weapon>> FindDamagedWeaponsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves weapons within a specified value range.
    /// </summary>
    /// <param name="minValue">Minimum weapon value</param>
    /// <param name="maxValue">Maximum weapon value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of weapons within the value range</returns>
    Task<IReadOnlyList<Domain.Weapon.Weapon>> FindByValueRangeAsync(decimal minValue, decimal maxValue, CancellationToken cancellationToken = default);
}
