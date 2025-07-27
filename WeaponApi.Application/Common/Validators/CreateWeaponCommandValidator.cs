using FluentValidation;
using WeaponApi.Application.Weapon.Commands;
using WeaponApi.Domain.Weapon;

namespace WeaponApi.Application.Common.Validators;

/// <summary>
/// FluentValidation validator for CreateWeaponCommand.
/// Provides comprehensive validation rules for weapon creation.
/// Follows Object Calisthenics principles with single responsibility.
/// </summary>
public sealed class CreateWeaponCommandValidator : AbstractValidator<CreateWeaponCommand>
{
    public CreateWeaponCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Weapon name is required")
            .Length(1, 100)
            .WithMessage("Weapon name must be between 1 and 100 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid weapon type specified");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Weapon description is required")
            .Length(1, 500)
            .WithMessage("Weapon description must be between 1 and 500 characters");

        RuleFor(x => x.HitPoints)
            .GreaterThan(0)
            .WithMessage("Hit points must be greater than zero")
            .LessThanOrEqualTo(10000)
            .WithMessage("Hit points cannot exceed 10,000");

        RuleFor(x => x.Damage)
            .GreaterThan(0)
            .WithMessage("Damage must be greater than zero")
            .LessThanOrEqualTo(1000)
            .WithMessage("Damage cannot exceed 1,000");

        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Weapon value cannot be negative")
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("Weapon value cannot exceed 1,000,000");
    }
}
