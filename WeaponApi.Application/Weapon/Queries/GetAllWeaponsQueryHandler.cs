using MediatR;
using WeaponApi.Application.Weapon;

namespace WeaponApi.Application.Weapon.Queries;

/// <summary>
/// Handles the GetAllWeaponsQuery to retrieve all weapons
/// Follows Object Calisthenics principles with single responsibility
/// </summary>
public sealed class GetAllWeaponsQueryHandler : IRequestHandler<GetAllWeaponsQuery, GetAllWeaponsQueryResult>
{
    private readonly IWeaponRepository repository;

    public GetAllWeaponsQueryHandler(IWeaponRepository repository)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetAllWeaponsQueryResult> Handle(GetAllWeaponsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var weapons = await repository.FindAllAsync(cancellationToken);
            var weaponDataList = weapons.Select(WeaponData.FromDomain).ToList().AsReadOnly();

            return GetAllWeaponsQueryResult.Success(weaponDataList);
        }
        catch (Exception ex)
        {
            return GetAllWeaponsQueryResult.Failure($"Failed to retrieve weapons: {ex.Message}");
        }
    }
}
