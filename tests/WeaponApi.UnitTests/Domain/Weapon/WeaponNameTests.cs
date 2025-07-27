using FluentAssertions;
using WeaponApi.Domain.Weapon;
using Xunit;

namespace WeaponApi.UnitTests.Domain.Weapon;

public sealed class WeaponNameTests
{
    [Fact]
    public void Create_WithValidName_ShouldCreateWeaponName()
    {
        // Arrange
        const string customName = "Excalibur";

        // Act
        var weaponName = WeaponName.Create(customName);

        // Assert
        weaponName.Should().NotBeNull();
        weaponName.Value.Should().Be(customName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Act
        var act = () => WeaponName.Create(invalidName);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Weapon name cannot be empty*")
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void Create_WithNullName_ShouldThrowArgumentException()
    {
        // Act
        var act = () => WeaponName.Create(null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Weapon name cannot be empty*")
            .And.ParamName.Should().Be("value");
    }

    [Fact]
    public void Generate_WithValidWeaponType_ShouldGenerateValidName()
    {
        // Arrange
        var weaponType = WeaponType.Sword;

        // Act
        var weaponName = WeaponName.Generate(weaponType);

        // Assert
        weaponName.Should().NotBeNull();
        weaponName.Value.Should().StartWith("Sword of ");
        weaponName.Value.Length.Should().BeGreaterThan(9); // "Sword of " + descriptor
    }

    [Fact]
    public void Generate_WithNullWeaponType_ShouldThrowArgumentNullException()
    {
        // Act
        var act = () => WeaponName.Generate(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("weaponType");
    }

    [Fact]
    public void Generate_WithValidWeaponTypeAndDescriptor_ShouldGenerateCorrectName()
    {
        // Arrange
        var weaponType = WeaponType.Staff;
        const string descriptor = "pudding";

        // Act
        var weaponName = WeaponName.Generate(weaponType, descriptor);

        // Assert
        weaponName.Should().NotBeNull();
        weaponName.Value.Should().Be("Staff of pudding");
    }

    [Fact]
    public void Generate_WithNullWeaponTypeAndDescriptor_ShouldThrowArgumentNullException()
    {
        // Arrange
        const string descriptor = "pudding";

        // Act
        var act = () => WeaponName.Generate(null!, descriptor);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("weaponType");
    }

    [Theory]
    [InlineData("")]
    public void Generate_WithInvalidDescriptor_ShouldThrowArgumentException(string invalidDescriptor)
    {
        // Arrange
        var weaponType = WeaponType.Sword;

        // Act
        var act = () => WeaponName.Generate(weaponType, invalidDescriptor);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Default descriptor cannot be null or empty*")
            .And.ParamName.Should().Be("defaultDescriptor");
    }

    [Fact]
    public void Generate_WithNullDescriptor_ShouldThrowArgumentException()
    {
        // Arrange
        var weaponType = WeaponType.Sword;

        // Act
        var act = () => WeaponName.Generate(weaponType, null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Default descriptor cannot be null or empty*")
            .And.ParamName.Should().Be("defaultDescriptor");
    }

    [Fact]
    public void Generate_WithUnknownDescriptor_ShouldThrowArgumentException()
    {
        // Arrange
        var weaponType = WeaponType.Sword;
        const string unknownDescriptor = "UnknownDescriptor";

        // Act
        var act = () => WeaponName.Generate(weaponType, unknownDescriptor);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Default descriptor must be one of the predefined descriptors*")
            .And.ParamName.Should().Be("defaultDescriptor");
    }

    [Fact]
    public void Generate_WithSeed_ShouldGenerateDeterministicName()
    {
        // Arrange
        var weaponType = WeaponType.Bow;
        const int seed = 42;

        // Act
        var weaponName1 = WeaponName.Generate(weaponType, seed);
        var weaponName2 = WeaponName.Generate(weaponType, seed);

        // Assert
        weaponName1.Value.Should().Be(weaponName2.Value);
        weaponName1.Value.Should().StartWith("Bow of ");
    }

    [Fact]
    public void Generate_WithNullWeaponTypeAndSeed_ShouldThrowArgumentNullException()
    {
        // Arrange
        const int seed = 42;

        // Act
        var act = () => WeaponName.Generate(null!, seed);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .And.ParamName.Should().Be("weaponType");
    }

    [Theory]
    [InlineData("pudding", true)]
    [InlineData("Dave", true)]
    [InlineData("Flames", true)]
    [InlineData("NonExistent", false)]
    [InlineData("", false)]
    public void ContainsDescriptor_ShouldReturnCorrectResult(string descriptor, bool expected)
    {
        // Arrange
        var weaponName = WeaponName.Generate(WeaponType.Sword, "pudding");

        // Act
        var result = weaponName.ContainsDescriptor(descriptor);

        // Assert
        result.Should().Be(expected && descriptor == "pudding");
    }

    [Fact]
    public void ContainsDescriptor_WithNullDescriptor_ShouldReturnFalse()
    {
        // Arrange
        var weaponName = WeaponName.Generate(WeaponType.Sword, "pudding");

        // Act
        var result = weaponName.ContainsDescriptor(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ContainsDescriptor_WithDifferentCasing_ShouldBeCaseInsensitive()
    {
        // Arrange
        var weaponName = WeaponName.Generate(WeaponType.Sword, "pudding");

        // Act
        var result = weaponName.ContainsDescriptor("PUDDING");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnValue()
    {
        // Arrange
        const string customName = "Legendary Sword";
        var weaponName = WeaponName.Create(customName);

        // Act
        var result = weaponName.ToString();

        // Assert
        result.Should().Be(customName);
    }

    [Fact]
    public void Equality_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        const string name = "Test Weapon";
        var weaponName1 = WeaponName.Create(name);
        var weaponName2 = WeaponName.Create(name);

        // Act & Assert
        weaponName1.Should().Be(weaponName2);
        weaponName1.GetHashCode().Should().Be(weaponName2.GetHashCode());
    }

    [Fact]
    public void Equality_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var weaponName1 = WeaponName.Create("Weapon One");
        var weaponName2 = WeaponName.Create("Weapon Two");

        // Act & Assert
        weaponName1.Should().NotBe(weaponName2);
    }
}
