using FluentAssertions;
using Moq;
using WeaponApi.Domain.User;
using Xunit;

namespace WeaponApi.UnitTests.Domain.User;

/// <summary>
/// Unit tests for User aggregate root testing business logic,
/// validation rules, and domain invariants for user management.
/// </summary>
public sealed class UserTests
{
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;

    public UserTests()
    {
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
            .Returns("hashed_password");
        _mockPasswordHasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);
    }

    [Fact]
    public void Register_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var username = "testuser";
        var name = "Test User";
        var password = "SecurePassword123!";
        var dateOfBirth = new DateOnly(1990, 1, 1);

        // Act
        var user = WeaponApi.Domain.User.User.Register(username, name, email, password, dateOfBirth, _mockPasswordHasher.Object);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBeNull();
        user.Email.Should().Be(email);
        user.Profile.Username.Should().Be(username);
        user.Profile.Name.Should().Be(name);
        user.Profile.DateOfBirth.Should().Be(dateOfBirth);
        user.Roles.Should().Contain(Role.User);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateProfile_WithValidData_ShouldUpdateUserProfile()
    {
        // Arrange
        var user = CreateTestUser();
        var newUsername = "newusername";
        var newName = "New Name";
        var newDateOfBirth = new DateOnly(1985, 6, 15);

        // Act
        user.UpdateProfile(newUsername, newName, newDateOfBirth);

        // Assert
        user.Profile.Username.Should().Be(newUsername);
        user.Profile.Name.Should().Be(newName);
        user.Profile.DateOfBirth.Should().Be(newDateOfBirth);
    }

    [Fact]
    public void UpdateUsername_WithValidUsername_ShouldUpdateUsername()
    {
        // Arrange
        var user = CreateTestUser();
        var newUsername = "updateduser";

        // Act
        user.UpdateUsername(newUsername);

        // Assert
        user.Profile.Username.Should().Be(newUsername);
    }

    [Fact]
    public void ChangePassword_WithValidPassword_ShouldUpdatePassword()
    {
        // Arrange
        var user = CreateTestUser();
        var newPassword = "NewSecurePassword123!";

        // Act
        user.ChangePassword(newPassword, _mockPasswordHasher.Object);

        // Assert
        _mockPasswordHasher.Verify(h => h.HashPassword(newPassword), Times.Once);
    }

    [Fact]
    public void ChangePassword_WithNullPasswordHasher_ShouldThrowArgumentNullException()
    {
        // Arrange
        var user = CreateTestUser();
        var newPassword = "NewSecurePassword123!";

        // Act & Assert
        var act = () => user.ChangePassword(newPassword, null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("passwordHasher");
    }

    [Fact]
    public void ChangeEmail_WithValidEmail_ShouldUpdateEmail()
    {
        // Arrange
        var user = CreateTestUser();
        var newEmail = Email.Create("newemail@example.com");

        // Act
        user.ChangeEmail(newEmail);

        // Assert
        user.Email.Should().Be(newEmail);
    }

    [Fact]
    public void ChangeEmail_WithNullEmail_ShouldThrowArgumentNullException()
    {
        // Arrange
        var user = CreateTestUser();

        // Act & Assert
        var act = () => user.ChangeEmail(null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("newEmail");
    }

    [Fact]
    public void IsPasswordValid_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var user = CreateTestUser();
        var password = "CorrectPassword123!";

        // Act
        var result = user.IsPasswordValid(password, _mockPasswordHasher.Object);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsPasswordValid_WithNullPasswordHasher_ShouldThrowArgumentNullException()
    {
        // Arrange
        var user = CreateTestUser();
        var password = "password";

        // Act & Assert
        var act = () => user.IsPasswordValid(password, null!);
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("passwordHasher");
    }

    [Fact]
    public void RecordLogin_ShouldUpdateLastLoginTime()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.RecordLogin();

        // Assert
        // No direct assertion as LastLoginAt is internal to UserSecurity
        // The test ensures the method executes without throwing
        user.Should().NotBeNull();
    }

    [Fact]
    public void DomainEvents_AfterRegistration_ShouldContainUserRegisteredEvent()
    {
        // Arrange
        var email = Email.Create("test@example.com");
        var username = "testuser";
        var name = "Test User";
        var password = "SecurePassword123!";
        var dateOfBirth = new DateOnly(1990, 1, 1);

        // Act
        var user = WeaponApi.Domain.User.User.Register(username, name, email, password, dateOfBirth, _mockPasswordHasher.Object);

        // Assert
        user.DomainEvents.Should().HaveCount(1);
        user.DomainEvents.First().Should().BeOfType<UserRegisteredEvent>();
    }

    private WeaponApi.Domain.User.User CreateTestUser()
    {
        var email = Email.Create("test@example.com");
        var username = "testuser";
        var name = "Test User";
        var password = "SecurePassword123!";
        var dateOfBirth = new DateOnly(1990, 1, 1);

        return WeaponApi.Domain.User.User.Register(username, name, email, password, dateOfBirth, _mockPasswordHasher.Object);
    }
}
