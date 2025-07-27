using FluentAssertions;
using WeaponApi.Domain.Weapon;
using Xunit;
using static WeaponApi.Domain.Weapon.WeaponType;

namespace WeaponApi.UnitTests.Domain.Weapon;

public sealed class WeaponTypeTests
{
    [Fact]
    public void Create_WithValidEnum_ShouldCreateWeaponType()
    {
        // Act
        var weaponType = WeaponType.Create(WeaponTypeEnum.Sword);

        // Assert
        weaponType.Should().NotBeNull();
        weaponType.Value.Should().Be(WeaponTypeEnum.Sword);
    }

    [Fact]
    public void Create_WithInvalidEnum_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidEnum = (WeaponTypeEnum)999;

        // Act
        var act = () => WeaponType.Create(invalidEnum);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Invalid weapon type value*")
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void StaticProperties_ShouldReturnCorrectWeaponTypes()
    {
        // Act & Assert
        WeaponType.Sword.Value.Should().Be(WeaponTypeEnum.Sword);
        WeaponType.Axe.Value.Should().Be(WeaponTypeEnum.Axe);
        WeaponType.Bow.Value.Should().Be(WeaponTypeEnum.Bow);
        WeaponType.Staff.Value.Should().Be(WeaponTypeEnum.Staff);
        WeaponType.Spoon.Value.Should().Be(WeaponTypeEnum.Spoon);
    }

    [Theory]
    [InlineData(WeaponTypeEnum.Sword, true)]
    [InlineData(WeaponTypeEnum.Axe, true)]
    [InlineData(WeaponTypeEnum.Dagger, true)]
    [InlineData(WeaponTypeEnum.Mace, true)]
    [InlineData(WeaponTypeEnum.Spear, true)]
    [InlineData(WeaponTypeEnum.Bow, false)]
    [InlineData(WeaponTypeEnum.Crossbow, false)]
    [InlineData(WeaponTypeEnum.Sling, false)]
    public void IsMeleeWeapon_ShouldReturnCorrectValue(WeaponTypeEnum weaponEnum, bool expected)
    {
        // Arrange
        var weaponType = WeaponType.Create(weaponEnum);

        // Act
        var result = weaponType.IsMeleeWeapon();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(WeaponTypeEnum.Bow, true)]
    [InlineData(WeaponTypeEnum.Crossbow, true)]
    [InlineData(WeaponTypeEnum.Sling, true)]
    [InlineData(WeaponTypeEnum.Staff, true)]
    [InlineData(WeaponTypeEnum.Wand, true)]
    [InlineData(WeaponTypeEnum.Sword, false)]
    [InlineData(WeaponTypeEnum.Axe, false)]
    [InlineData(WeaponTypeEnum.Dagger, false)]
    public void IsRangedWeapon_ShouldReturnCorrectValue(WeaponTypeEnum weaponEnum, bool expected)
    {
        // Arrange
        var weaponType = WeaponType.Create(weaponEnum);

        // Act
        var result = weaponType.IsRangedWeapon();

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(WeaponTypeEnum.Staff, true)]
    [InlineData(WeaponTypeEnum.Wand, true)]
    [InlineData(WeaponTypeEnum.Scythe, true)]
    [InlineData(WeaponTypeEnum.Spoon, true)]
    [InlineData(WeaponTypeEnum.Sponge, true)]
    [InlineData(WeaponTypeEnum.Sword, false)]
    [InlineData(WeaponTypeEnum.Bow, false)]
    [InlineData(WeaponTypeEnum.Axe, false)]
    public void IsMagicWeapon_ShouldReturnCorrectValue(WeaponTypeEnum weaponEnum, bool expected)
    {
        // Arrange
        var weaponType = WeaponType.Create(weaponEnum);

        // Act
        var result = weaponType.IsMagicWeapon();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ToString_ShouldReturnEnumString()
    {
        // Arrange
        var weaponType = WeaponType.Sword;

        // Act
        var result = weaponType.ToString();

        // Assert
        result.Should().Be("Sword");
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var weaponType1 = WeaponType.Create(WeaponTypeEnum.Sword);
        var weaponType2 = WeaponType.Create(WeaponTypeEnum.Sword);

        // Act & Assert
        weaponType1.Should().BeEquivalentTo(weaponType2);
        weaponType1.Value.Should().Be(weaponType2.Value);
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var weaponType1 = WeaponType.Sword;
        var weaponType2 = WeaponType.Axe;

        // Act & Assert
        weaponType1.Should().NotBe(weaponType2);
    }
}
