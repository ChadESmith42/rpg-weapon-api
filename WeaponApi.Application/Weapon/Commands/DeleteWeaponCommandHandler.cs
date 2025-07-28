using MediatR;
using WeaponApi.Application.Weapon;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Handles the DeleteWeaponCommand to remove a weapon from the system
/// Follows Object Calisthenics principles with single responsibility
/// </summary>
public sealed class DeleteWeaponCommandHandler : IRequestHandler<DeleteWeaponCommand, DeleteWeaponCommandResult>
{
    private readonly IWeaponRepository repository;

    public DeleteWeaponCommandHandler(IWeaponRepository repository)
    {
        this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<DeleteWeaponCommandResult> Handle(DeleteWeaponCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateRequest(request);
        if (!validationResult.IsValid)
        {
            return DeleteWeaponCommandResult.Failure(validationResult.ErrorMessage);
        }

        return await RemoveWeapon(request.WeaponId, cancellationToken);
    }

    private RequestValidationResult ValidateRequest(DeleteWeaponCommand request)
    {
        if (request?.WeaponId == null)
        {
            return RequestValidationResult.Invalid("Weapon ID is required");
        }

        return RequestValidationResult.Valid();
    }

    private async Task<DeleteWeaponCommandResult> RemoveWeapon(WeaponId weaponId, CancellationToken cancellationToken)
    {
        try
        {
            var weapon = await repository.FindByIdAsync(weaponId, cancellationToken);
            if (weapon == null)
            {
                return DeleteWeaponCommandResult.NotFound(weaponId);
            }

            await repository.RemoveAsync(weapon, cancellationToken);
            return DeleteWeaponCommandResult.Success();
        }
        catch (Exception ex)
        {
            return DeleteWeaponCommandResult.Failure($"Failed to delete weapon: {ex.Message}");
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
