using FluentAssertions;
using WeaponApi.Domain.Weapon;
using Xunit;

namespace WeaponApi.UnitTests.Domain.Weapon;

public sealed class RepairEstimateTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldCreateRepairEstimate()
    {
        // Arrange
        const int repairCost = 100;
        const int gainedHitPoints = 50;
        const decimal gainedValue = 25.75m;

        // Act
        var repairEstimate = new RepairEstimate(repairCost, gainedHitPoints, gainedValue);

        // Assert
        repairEstimate.Should().NotBeNull();
        repairEstimate.RepairCost.Should().Be(repairCost);
        repairEstimate.GainedHitPoints.Should().Be(gainedHitPoints);
        repairEstimate.GainedValue.Should().Be(gainedValue);
    }

    [Fact]
    public void Constructor_WithNegativeRepairCost_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int negativeRepairCost = -10;
        const int gainedHitPoints = 50;
        const decimal gainedValue = 25.75m;

        // Act
        var act = () => new RepairEstimate(negativeRepairCost, gainedHitPoints, gainedValue);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Repair cost cannot be negative.*")
            .And.ParamName.Should().Be("repairCost");
    }

    [Fact]
    public void Constructor_WithNegativeGainedHitPoints_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int repairCost = 100;
        const int negativeGainedHitPoints = -5;
        const decimal gainedValue = 25.75m;

        // Act
        var act = () => new RepairEstimate(repairCost, negativeGainedHitPoints, gainedValue);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Gained hit points cannot be negative.*")
            .And.ParamName.Should().Be("gainedHitPoints");
    }

    [Fact]
    public void Constructor_WithNegativeGainedValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        const int repairCost = 100;
        const int gainedHitPoints = 50;
        const decimal negativeGainedValue = -10.5m;

        // Act
        var act = () => new RepairEstimate(repairCost, gainedHitPoints, negativeGainedValue);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Gained value cannot be negative.*")
            .And.ParamName.Should().Be("gainedValue");
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(100, 50, 25.75)]
    [InlineData(1000, 500, 250.99)]
    public void Constructor_WithValidBoundaryValues_ShouldCreateRepairEstimate(int repairCost, int gainedHitPoints, decimal gainedValue)
    {
        // Act
        var repairEstimate = new RepairEstimate(repairCost, gainedHitPoints, gainedValue);

        // Assert
        repairEstimate.RepairCost.Should().Be(repairCost);
        repairEstimate.GainedHitPoints.Should().Be(gainedHitPoints);
        repairEstimate.GainedValue.Should().Be(gainedValue);
    }
}
