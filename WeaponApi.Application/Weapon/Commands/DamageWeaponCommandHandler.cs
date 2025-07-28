using MediatR;
using WeaponApi.Application.Weapon;
using WeaponApi.Application.Weapon.Commands;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Handler for DamageWeaponCommand.
/// Applies damage to a weapon and updates its state in the repository.
/// </summary>
public sealed class DamageWeaponCommandHandler : IRequestHandler<DamageWeaponCommand, DamageWeaponCommandResult>
{
    private readonly IWeaponRepository weaponRepository;

    public DamageWeaponCommandHandler(IWeaponRepository weaponRepository)
    {
        this.weaponRepository = weaponRepository ?? throw new ArgumentNullException(nameof(weaponRepository));
    }

    public async Task<DamageWeaponCommandResult> Handle(DamageWeaponCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.DamageAmount < 0)
            {
                return DamageWeaponCommandResult.Failure("Damage amount cannot be negative");
            }

            var weapon = await weaponRepository.FindByIdAsync(request.WeaponId);
            if (weapon == null)
            {
                return DamageWeaponCommandResult.NotFound(request.WeaponId);
            }

            weapon.DamageWeapon(request.DamageAmount);
            await weaponRepository.UpdateAsync(weapon);

            return DamageWeaponCommandResult.Success();
        }
        catch (Exception ex)
        {
            return DamageWeaponCommandResult.Failure($"Failed to damage weapon: {ex.Message}");
        }
    }
}
