using MediatR;

namespace WeaponApi.Application.Weapon.Queries;

/// <summary>
/// Query to retrieve all weapons in the system.
/// Follows MediatR CQRS pattern for query handling.
/// </summary>
public sealed record GetAllWeaponsQuery : IRequest<GetAllWeaponsQueryResult>;

/// <summary>
/// Result type for GetAllWeaponsQuery execution.
/// Provides structured response with weapons list or error information.
/// </summary>
public sealed class GetAllWeaponsQueryResult
{
    public bool IsSuccess { get; }
    public IReadOnlyList<WeaponData>? Weapons { get; }
    public string? ErrorMessage { get; }

    private GetAllWeaponsQueryResult(bool isSuccess, IReadOnlyList<WeaponData>? weapons = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        Weapons = weapons;
        ErrorMessage = errorMessage;
    }

    public static GetAllWeaponsQueryResult Success(IReadOnlyList<WeaponData> weapons) =>
        new(true, weapons);

    public static GetAllWeaponsQueryResult Failure(string errorMessage) =>
        new(false, errorMessage: errorMessage);
}
