using MediatR;
using WeaponApi.Application.Weapon.Queries;

namespace WeaponApi.Application.Weapon.Queries;

/// <summary>
/// Handler for EstimateRepairQuery.
/// Calculates repair estimates for a weapon without actually performing the repair.
/// </summary>
public sealed class EstimateRepairQueryHandler : IRequestHandler<EstimateRepairQuery, EstimateRepairQueryResult>
{
    public Task<EstimateRepairQueryResult> Handle(EstimateRepairQuery request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.RepairAmount < 0)
            {
                return Task.FromResult(EstimateRepairQueryResult.Failure("Repair amount cannot be negative"));
            }

            if (request.Weapon == null)
            {
                return Task.FromResult(EstimateRepairQueryResult.Failure("Weapon cannot be null"));
            }

            var estimate = request.Weapon.GetRepairEstimate(request.RepairAmount);
            return Task.FromResult(EstimateRepairQueryResult.Success(estimate));
        }
        catch (InvalidOperationException ex)
        {
            return Task.FromResult(EstimateRepairQueryResult.Failure(ex.Message));
        }
        catch (Exception ex)
        {
            return Task.FromResult(EstimateRepairQueryResult.Failure($"Failed to estimate repair: {ex.Message}"));
        }
    }
}
