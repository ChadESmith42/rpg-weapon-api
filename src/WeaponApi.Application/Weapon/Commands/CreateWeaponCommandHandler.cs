using MediatR;
using WeaponApi.Application.Weapon.Commands;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Weapon.Commands;

/// <summary>
/// Handler for CreateWeaponCommand that orchestrates weapon creation business logic.
/// Implements MediatR IRequestHandler for CQRS pattern.
/// </summary>
public sealed class CreateWeaponCommandHandler : IRequestHandler<CreateWeaponCommand, CreateWeaponCommandResult>
{
    private readonly IWeaponRepository weaponRepository;

    public CreateWeaponCommandHandler(IWeaponRepository weaponRepository)
    {
        this.weaponRepository = weaponRepository ?? throw new ArgumentNullException(nameof(weaponRepository));
    }

    public async Task<CreateWeaponCommandResult> Handle(CreateWeaponCommand request, CancellationToken cancellationToken)
    {
        var validationResult = ValidateCommand(request);
        if (!validationResult.IsValid)
            return CreateWeaponCommandResult.ValidationFailure(validationResult.Errors);

        var weaponCreationResult = await CreateWeaponFromCommand(request, cancellationToken);
        return weaponCreationResult;
    }

    private async Task<CreateWeaponCommandResult> CreateWeaponFromCommand(CreateWeaponCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var weaponType = WeaponType.Create(request.Type);
            var weaponName = WeaponName.Generate(weaponType, request.Name);

            var duplicateCheckResult = await CheckForDuplicateWeapon(weaponName, cancellationToken);
            if (!duplicateCheckResult.IsValid)
                return CreateWeaponCommandResult.ValidationFailure(duplicateCheckResult.ErrorMessage);

            var weapon = CreateWeaponFromValidatedRequest(request, weaponName);
            await weaponRepository.AddAsync(weapon, cancellationToken);

            return CreateWeaponCommandResult.Success(weapon.Id);
        }
        catch (ArgumentException ex)
        {
            return CreateWeaponCommandResult.ValidationFailure(ex.Message);
        }
        catch (Exception ex)
        {
            return CreateWeaponCommandResult.Failure($"An error occurred while creating the weapon: {ex.Message}");
        }
    }

    private static Domain.Weapon.Weapon CreateWeaponFromValidatedRequest(CreateWeaponCommand request, WeaponName weaponName)
    {
        return Domain.Weapon.Weapon.Create(
            weaponName,
            request.Description,
            request.HitPoints,
            request.Damage,
            request.IsRepairable,
            request.Value
        );
    }

    private async Task<DuplicateCheckResult> CheckForDuplicateWeapon(WeaponName weaponName, CancellationToken cancellationToken)
    {
        var existingWeapon = await weaponRepository.FindByNameAsync(weaponName, cancellationToken);
        if (existingWeapon != null)
            return DuplicateCheckResult.Duplicate($"A weapon with the name '{weaponName}' already exists");

        return DuplicateCheckResult.Valid();
    }

    private static ValidationResult ValidateCommand(CreateWeaponCommand command)
    {
        var validator = new WeaponCommandValidator();
        return validator.Validate(command);
    }
}

/// <summary>
/// Validation result for weapon command validation.
/// </summary>
public sealed class ValidationResult
{
    public bool IsValid { get; }
    public IReadOnlyList<string> Errors { get; }

    private ValidationResult(bool isValid, IReadOnlyList<string> errors)
    {
        IsValid = isValid;
        Errors = errors;
    }

    public static ValidationResult Valid() => new(true, Array.Empty<string>());
    public static ValidationResult Invalid(IReadOnlyList<string> errors) => new(false, errors);
}

/// <summary>
/// Result for duplicate weapon name checking.
/// </summary>
public sealed class DuplicateCheckResult
{
    public bool IsValid { get; }
    public string ErrorMessage { get; }

    private DuplicateCheckResult(bool isValid, string errorMessage = "")
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static DuplicateCheckResult Valid() => new(true);
    public static DuplicateCheckResult Duplicate(string message) => new(false, message);
}

/// <summary>
/// Weapon command validator following Object Calisthenics principles.
/// </summary>
public sealed class WeaponCommandValidator
{
    public ValidationResult Validate(CreateWeaponCommand command)
    {
        var errors = new List<string>();

        ValidateWeaponName(command.Name, errors);
        ValidateWeaponDescription(command.Description, errors);
        ValidateHitPoints(command.HitPoints, errors);
        ValidateDamage(command.Damage, errors);
        ValidateValue(command.Value, errors);
        ValidateWeaponType(command.Type, errors);

        return errors.Any()
            ? ValidationResult.Invalid(errors.AsReadOnly())
            : ValidationResult.Valid();
    }

    private static void ValidateWeaponName(string name, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add("Weapon name is required");
            return;
        }

        if (name.Length > 100)
            errors.Add("Weapon name cannot exceed 100 characters");
    }

    private static void ValidateWeaponDescription(string description, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            errors.Add("Weapon description is required");
            return;
        }

        if (description.Length > 500)
            errors.Add("Weapon description cannot exceed 500 characters");
    }

    private static void ValidateHitPoints(int hitPoints, List<string> errors)
    {
        if (hitPoints <= 0)
        {
            errors.Add("Weapon hit points must be greater than 0");
            return;
        }

        if (hitPoints > 10000)
            errors.Add("Weapon hit points cannot exceed 10,000");
    }

    private static void ValidateDamage(int damage, List<string> errors)
    {
        if (damage < 0)
        {
            errors.Add("Weapon damage cannot be negative");
            return;
        }

        if (damage > 1000)
            errors.Add("Weapon damage cannot exceed 1,000");
    }

    private static void ValidateValue(decimal value, List<string> errors)
    {
        if (value < 0)
        {
            errors.Add("Weapon value cannot be negative");
            return;
        }

        if (value > 1_000_000)
            errors.Add("Weapon value cannot exceed 1,000,000");
    }

    private static void ValidateWeaponType(WeaponTypeEnum type, List<string> errors)
    {
        if (!Enum.IsDefined(typeof(WeaponTypeEnum), type))
            errors.Add("Invalid weapon type specified");
    }
}
