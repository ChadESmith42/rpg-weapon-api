using MediatR;
using WeaponApi.Application.Weapon;
using WeaponApi.Application.Weapon.Commands;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Handler for RepairWeaponCommand.
/// Repairs a weapon and updates its state in the repository.
/// </summary>
public sealed class RepairWeaponCommandHandler : IRequestHandler<RepairWeaponCommand, RepairWeaponCommandResult>
{
    private readonly IWeaponRepository weaponRepository;

    public RepairWeaponCommandHandler(IWeaponRepository weaponRepository)
    {
        this.weaponRepository = weaponRepository ?? throw new ArgumentNullException(nameof(weaponRepository));
    }

    public async Task<RepairWeaponCommandResult> Handle(RepairWeaponCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.RepairAmount < 0)
            {
                return RepairWeaponCommandResult.Failure("Repair amount cannot be negative");
            }

            var weapon = await weaponRepository.FindByIdAsync(request.WeaponId);
            if (weapon == null)
            {
                return RepairWeaponCommandResult.NotFound(request.WeaponId);
            }

            weapon.RepairWeapon(request.RepairAmount);
            await weaponRepository.UpdateAsync(weapon);

            return RepairWeaponCommandResult.Success();
        }
        catch (InvalidOperationException ex)
        {
            return RepairWeaponCommandResult.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return RepairWeaponCommandResult.Failure($"Failed to repair weapon: {ex.Message}");
        }
    }
}
