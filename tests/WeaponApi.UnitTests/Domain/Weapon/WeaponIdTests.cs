using FluentAssertions;
using WeaponApi.Domain.Weapon;
using Xunit;

namespace WeaponApi.UnitTests.Domain.Weapon;

public sealed class WeaponIdTests
{
    [Fact]
    public void Create_ShouldGenerateValidWeaponId()
    {
        // Act
        var weaponId = WeaponId.Create();

        // Assert
        weaponId.Should().NotBeNull();
        weaponId.Value.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Create_WithValidGuid_ShouldCreateWeaponId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var weaponId = WeaponId.Create(guid);

        // Assert
        weaponId.Should().NotBeNull();
        weaponId.Value.Should().Be(guid);
    }

    [Fact]
    public void Create_WithEmptyGuid_ShouldThrowArgumentException()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var act = () => WeaponId.Create(emptyGuid);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("WeaponId cannot be empty*")
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void ImplicitConversion_ToGuid_ShouldReturnValue()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var weaponId = WeaponId.Create(guid);

        // Act
        Guid convertedGuid = weaponId;

        // Assert
        convertedGuid.Should().Be(guid);
    }

    [Fact]
    public void ToString_ShouldReturnGuidString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var weaponId = WeaponId.Create(guid);

        // Act
        var result = weaponId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var weaponId1 = WeaponId.Create(guid);
        var weaponId2 = WeaponId.Create(guid);

        // Act & Assert
        weaponId1.Should().Be(weaponId2);
        weaponId1.GetHashCode().Should().Be(weaponId2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var weaponId1 = WeaponId.Create();
        var weaponId2 = WeaponId.Create();

        // Act & Assert
        weaponId1.Should().NotBe(weaponId2);
    }
}
