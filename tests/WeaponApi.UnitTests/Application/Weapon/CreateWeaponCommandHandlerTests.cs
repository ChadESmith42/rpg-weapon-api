using FluentAssertions;
using Moq;
using WeaponApi.Application.Weapon;
using WeaponApi.Application.Weapon.Commands;
using WeaponApi.Domain.Weapon;
using Xunit;

namespace WeaponApi.UnitTests.Application.Weapon;

/// <summary>
/// Unit tests for CreateWeaponCommandHandler testing the command handling logic,
/// validation, and repository interaction for weapon creation.
/// </summary>
public sealed class CreateWeaponCommandHandlerTests
{
    private readonly Mock<IWeaponRepository> _mockRepository;
    private readonly CreateWeaponCommandHandler _handler;

    public CreateWeaponCommandHandlerTests()
    {
        _mockRepository = new Mock<IWeaponRepository>();
        _handler = new CreateWeaponCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldCreateWeaponAndReturnId()
    {
        // Arrange
        var command = new CreateWeaponCommand(
            "Flames", // Use a valid descriptor instead of full name
            WeaponType.WeaponTypeEnum.Sword,
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        _mockRepository
            .Setup(r => r.FindByNameAsync(It.IsAny<WeaponName>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeaponApi.Domain.Weapon.Weapon?)null); // No duplicate weapon found

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        if (!result.IsSuccess)
        {
            // Debug output to see why it's failing
            var debugInfo = $"Validation Errors: [{string.Join(", ", result.ValidationErrors)}], Error Message: {result.ErrorMessage}";
            throw new InvalidOperationException($"Test failed: {debugInfo}");
        }
        result.IsSuccess.Should().BeTrue();
        result.WeaponId.Should().NotBeNull();

        _mockRepository.Verify(
            r => r.AddAsync(It.Is<WeaponApi.Domain.Weapon.Weapon>(w =>
                w.Name.Value == "Flames" &&
                w.Description == "A mighty test sword"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidWeaponName_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateWeaponCommand(
            "", // Invalid empty name
            WeaponType.WeaponTypeEnum.Sword,
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().Contain(error => error.Contains("name"));

        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithNegativeHitPoints_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateWeaponCommand(
            "Lightning", // Use valid descriptor
            WeaponType.WeaponTypeEnum.Sword,
            "A mighty test sword",
            -10, // Invalid negative hit points
            25,
            true,
            150.0m
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().Contain(error => error.Contains("hit points"));

        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithNegativeDamage_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateWeaponCommand(
            "Darkness", // Use valid descriptor
            WeaponType.WeaponTypeEnum.Sword,
            "A mighty test sword",
            100,
            -5, // Invalid negative damage
            true,
            150.0m
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().Contain(error => error.Contains("damage"));

        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithNegativeValue_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateWeaponCommand(
            "Power", // Use valid descriptor
            WeaponType.WeaponTypeEnum.Sword,
            "A mighty test sword",
            100,
            25,
            true,
            -50.0m // Invalid negative value
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().Contain(error => error.Contains("value"));

        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WithRepositoryException_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new CreateWeaponCommand(
            "Pudding", // Use a valid descriptor
            WeaponType.WeaponTypeEnum.Sword,
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Database connection failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Database connection failed");
    }

    [Fact]
    public async Task Handle_WithCancellationToken_ShouldPassTokenToRepository()
    {
        // Arrange
        var command = new CreateWeaponCommand(
            "Ice", // Use a valid descriptor
            WeaponType.WeaponTypeEnum.Sword,
            "A mighty test sword",
            100,
            25,
            true,
            150.0m
        );

        using var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;

        _mockRepository
            .Setup(r => r.FindByNameAsync(It.IsAny<WeaponName>(), cancellationToken))
            .ReturnsAsync((WeaponApi.Domain.Weapon.Weapon?)null); // No duplicate weapon found

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WithMaximumValues_ShouldCreateWeaponSuccessfully()
    {
        // Arrange
        var command = new CreateWeaponCommand(
            "Lightning", // Use a valid descriptor that's 100 chars or less
            WeaponType.WeaponTypeEnum.Sword,
            new string('B', 500),  // Long description
            10000,                 // Maximum hit points (per validation rules)
            1000,                  // Maximum damage (per validation rules)
            false,                 // Not repairable
            1_000_000m             // Maximum value (per validation rules)
        );

        _mockRepository
            .Setup(r => r.FindByNameAsync(It.IsAny<WeaponName>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WeaponApi.Domain.Weapon.Weapon?)null); // No duplicate weapon found

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _mockRepository.Verify(
            r => r.AddAsync(It.IsAny<WeaponApi.Domain.Weapon.Weapon>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
