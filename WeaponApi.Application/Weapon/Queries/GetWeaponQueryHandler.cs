using MediatR;
using WeaponApi.Application.Weapon;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Queries;

/// <summary>
/// Handles the GetWeaponQuery to retrieve a weapon by its ID
/// Follows Object Calisthenics principles with single responsibility
/// </summary>
public sealed class GetWeaponQueryHandler : IRequestHandler<GetWeaponQuery, GetWeaponQueryResult>
{
    private readonly IWeaponRepository repository;

    public GetWeaponQueryHandler(IWeaponRepository repository)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<GetWeaponQueryResult> Handle(GetWeaponQuery request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateRequest(request);
        if (!validationResult.IsValid)
        {
            return GetWeaponQueryResult.Failure(validationResult.ErrorMessage);
        }

        return await RetrieveWeapon(request.WeaponId);
    }

    private RequestValidationResult ValidateRequest(GetWeaponQuery request)
    {
        if (request?.WeaponId == null)
        {
            return RequestValidationResult.Invalid("Weapon ID is required");
        }

        return RequestValidationResult.Valid();
    }

    private async Task<GetWeaponQueryResult> RetrieveWeapon(WeaponId weaponId)
    {
        try
        {
            var weapon = await repository.FindByIdAsync(weaponId);
            if (weapon == null)
            {
                return GetWeaponQueryResult.NotFound(weaponId);
            }

            var weaponData = WeaponData.FromDomain(weapon);
            return GetWeaponQueryResult.Success(weaponData);
        }
        catch (Exception ex)
        {
            return GetWeaponQueryResult.Failure($"Failed to retrieve weapon: {ex.Message}");
        }
    }
}

/// <summary>
/// Value object for request validation results
/// Encapsulates validation state without exposing implementation details
/// </summary>
public sealed record RequestValidationResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    private RequestValidationResult(bool isValid, string errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage ?? string.Empty;
    }

    public static RequestValidationResult Valid() => new(true, string.Empty);
    public static RequestValidationResult Invalid(string errorMessage) => new(false, errorMessage);
}
