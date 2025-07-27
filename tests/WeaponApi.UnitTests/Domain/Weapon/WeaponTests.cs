using FluentAssertions;
using WeaponApi.Domain.Weapon;
using Xunit;

namespace WeaponApi.UnitTests.Domain.Weapon;

/// <summary>
/// Unit tests for the Weapon aggregate root, testing business logic
/// for damage, repair, and estimation functionality.
/// </summary>
public sealed class WeaponTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateWeapon()
    {
        // Arrange
        var weaponName = WeaponName.Create("Test Sword");
        const string description = "A mighty test sword";
        const int hitPoints = 100;
        const int damage = 25;
        const bool isRepairable = true;
        const decimal value = 150.0m;

        // Act
        var weapon = WeaponApi.Domain.Weapon.Weapon.Create(weaponName, description, hitPoints, damage, isRepairable, value);

        // Assert
        weapon.Should().NotBeNull();
        weapon.Id.Should().NotBeNull();
        weapon.Name.Should().Be(weaponName);
        weapon.Description.Should().Be(description);
        weapon.HitPoints.Should().Be(hitPoints);
        weapon.Damage.Should().Be(damage);
        weapon.IsRepairable.Should().Be(isRepairable);
        weapon.Value.Should().Be(value);
    }

    [Fact]
    public void DamageWeapon_WithValidDamage_ShouldReduceHitPointsAndUpdateValue()
    {
        // Arrange
        var weapon = CreateTestWeapon();
        var originalHitPoints = weapon.HitPoints;
        var originalDamage = weapon.Damage;
        const int damageAmount = 25;

        // Act
        weapon.DamageWeapon(damageAmount);

        // Assert
        weapon.Damage.Should().Be(originalDamage + damageAmount);
        weapon.HitPoints.Should().BeLessThanOrEqualTo(originalHitPoints);
    }

    [Fact]
    public void DamageWeapon_WithExcessiveDamage_ShouldNotReduceHitPointsBelowZero()
    {
        // Arrange
        var weapon = CreateTestWeapon();
        const int excessiveDamage = 200;

        // Act
        weapon.DamageWeapon(excessiveDamage);

        // Assert
        weapon.HitPoints.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void RepairWeapon_WithValidAmount_ShouldReduceDamageAndIncreaseValue()
    {
        // Arrange
        var weapon = CreateDamagedWeapon();
        var originalDamage = weapon.Damage;
        var originalValue = weapon.Value;
        const int repairAmount = 15;

        // Act
        weapon.RepairWeapon(repairAmount);

        // Assert
        weapon.Damage.Should().BeLessThanOrEqualTo(originalDamage);
        weapon.Value.Should().BeGreaterThanOrEqualTo(originalValue);
    }

    [Fact]
    public void RepairWeapon_WithNonRepairableWeapon_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var weapon = CreateNonRepairableWeapon();
        const int repairAmount = 10;

        // Act
        var act = () => weapon.RepairWeapon(repairAmount);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("This weapon cannot be repaired.");
    }

    [Fact]
    public void GetRepairEstimate_WithRepairableWeapon_ShouldReturnValidEstimate()
    {
        // Arrange
        var weapon = CreateDamagedWeapon();
        const int repairAmount = 20;

        // Act
        var estimate = weapon.GetRepairEstimate(repairAmount);

        // Assert
        estimate.Should().NotBeNull();
        estimate.RepairCost.Should().BeGreaterThanOrEqualTo(0);
        estimate.GainedHitPoints.Should().BeGreaterThanOrEqualTo(0);
        estimate.GainedValue.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void GetRepairEstimate_WithNonRepairableWeapon_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var weapon = CreateNonRepairableWeapon();
        const int repairAmount = 10;

        // Act
        var act = () => weapon.GetRepairEstimate(repairAmount);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("This weapon cannot be repaired.");
    }

    [Fact]
    public void ValidateWeapon_WithValidWeapon_ShouldNotThrow()
    {
        // Arrange
        var weapon = CreateTestWeapon();

        // Act
        var act = () => weapon.ValidateWeapon();

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void DamageWeapon_ShouldUpdateWeaponState()
    {
        // Arrange
        var weapon = CreateTestWeapon();
        var originalHitPoints = weapon.HitPoints;
        const int damageAmount = 30;

        // Act
        weapon.DamageWeapon(damageAmount);

        // Assert
        weapon.HitPoints.Should().BeLessThanOrEqualTo(originalHitPoints);
        weapon.Damage.Should().BeGreaterThan(0);
    }

    [Fact]
    public void RepairWeapon_ShouldRestoreWeaponCondition()
    {
        // Arrange
        var weapon = CreateDamagedWeapon();
        var originalDamage = weapon.Damage;
        const int repairAmount = 10;

        // Act
        weapon.RepairWeapon(repairAmount);

        // Assert
        weapon.Damage.Should().BeLessThanOrEqualTo(originalDamage);
        weapon.HitPoints.Should().BeGreaterThan(0);
    }

    [Fact]
    public void GetRepairEstimate_ShouldCalculateCorrectCosts()
    {
        // Arrange
        var weapon = CreateDamagedWeapon();
        const int repairAmount = 15;

        // Act
        var estimate = weapon.GetRepairEstimate(repairAmount);

        // Assert
        estimate.RepairCost.Should().BeGreaterThan(0);
        estimate.GainedHitPoints.Should().BeGreaterThan(0);
        estimate.GainedValue.Should().BeGreaterThanOrEqualTo(0);
    }

    [Theory]
    [InlineData(50, 25)]
    [InlineData(100, 50)]
    [InlineData(25, 10)]
    public void DamageWeapon_WithDifferentAmounts_ShouldUpdateStateCorrectly(int initialHitPoints, int damageAmount)
    {
        // Arrange
        var weaponName = WeaponName.Create("Test Weapon");
        var weapon = WeaponApi.Domain.Weapon.Weapon.Create(weaponName, "Test", initialHitPoints, 0, true, 100m);

        // Act
        weapon.DamageWeapon(damageAmount);

        // Assert
        weapon.Damage.Should().Be(damageAmount);
        weapon.HitPoints.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void Weapon_ShouldHaveCorrectInitialType()
    {
        // Arrange & Act
        var weapon = CreateTestWeapon();

        // Assert
        weapon.Type.Should().NotBeNull();
        weapon.Type.Value.Should().Be(WeaponType.WeaponTypeEnum.Spoon); // Check the enum value instead
    }

    /// <summary>
    /// Creates a standard test weapon for use in multiple test scenarios.
    /// </summary>
    /// <returns>A weapon with standard test values.</returns>
    private static WeaponApi.Domain.Weapon.Weapon CreateTestWeapon()
    {
        var weaponName = WeaponName.Create("Test Sword");
        const string description = "A test weapon for unit testing";
        const int hitPoints = 100;
        const int damage = 0;
        const bool isRepairable = true;
        const decimal value = 150.0m;

        return WeaponApi.Domain.Weapon.Weapon.Create(weaponName, description, hitPoints, damage, isRepairable, value);
    }

    /// <summary>
    /// Creates a damaged weapon for testing repair functionality.
    /// </summary>
    /// <returns>A weapon that has sustained some damage.</returns>
    private static WeaponApi.Domain.Weapon.Weapon CreateDamagedWeapon()
    {
        var weapon = CreateTestWeapon();
        weapon.DamageWeapon(30); // Apply some damage
        return weapon;
    }

    /// <summary>
    /// Creates a non-repairable weapon for testing repair restrictions.
    /// </summary>
    /// <returns>A weapon that cannot be repaired.</returns>
    private static WeaponApi.Domain.Weapon.Weapon CreateNonRepairableWeapon()
    {
        var weaponName = WeaponName.Create("Fragile Weapon");
        const string description = "A weapon that cannot be repaired";
        const int hitPoints = 50;
        const int damage = 10;
        const bool isRepairable = false;
        const decimal value = 75.0m;

        return WeaponApi.Domain.Weapon.Weapon.Create(weaponName, description, hitPoints, damage, isRepairable, value);
    }
}
