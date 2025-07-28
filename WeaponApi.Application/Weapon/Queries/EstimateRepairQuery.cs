using MediatR;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Queries;

/// <summary>
/// Query to get repair estimate for a weapon.
/// </summary>
public sealed record EstimateRepairQuery(
    Domain.Weapon.Weapon Weapon,
    int RepairAmount
) : IRequest<EstimateRepairQueryResult>;

/// <summary>
/// Result type for EstimateRepairQuery execution.
/// </summary>
public sealed class EstimateRepairQueryResult
{
    public bool IsSuccess { get; }
    public RepairEstimate? Estimate { get; }
    public string? ErrorMessage { get; }

    private EstimateRepairQueryResult(bool isSuccess, RepairEstimate? estimate = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        Estimate = estimate;
        ErrorMessage = errorMessage;
    }

    public static EstimateRepairQueryResult Success(RepairEstimate estimate) =>
        new(true, estimate);

    public static EstimateRepairQueryResult Failure(string errorMessage) =>
        new(false, errorMessage: errorMessage);
}
