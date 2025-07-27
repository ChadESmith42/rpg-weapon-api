using WeaponApi.Domain.Common;

namespace WeaponApi.Domain.User;

/// <summary>
/// User aggregate root representing a registered user in the system.
/// Handles authentication and profile management.
/// </summary>
public sealed class User
{
    private readonly List<IDomainEvent> domainEvents = new();

    public UserId Id { get; }
    public Email Email { get; private set; }
    public UserProfile Profile { get; private set; }
    public UserSecurity Security { get; private set; }
    public DateTime CreatedAt { get; }

    public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    private User(UserId id, Email email, UserProfile profile, UserSecurity security, DateTime createdAt)
    {
        Id = id;
        Email = email;
        Profile = profile;
        Security = security;
        CreatedAt = createdAt;
    }

    public static User Register(string username, string name, Email email, string password, DateOnly dateOfBirth, IPasswordHasher passwordHasher)
    {
        if (passwordHasher == null)
            throw new ArgumentNullException(nameof(passwordHasher));

        var id = UserId.Create();
        var profile = UserProfile.Create(username, name, dateOfBirth);
        var hashedPassword = passwordHasher.HashPassword(password);
        var security = UserSecurity.Create(hashedPassword);
        var createdAt = DateTime.UtcNow;

        var user = new User(id, email, profile, security, createdAt);
        user.AddDomainEvent(new UserRegisteredEvent(id, email));

        return user;
    }

    public static User Create(UserId id, Email email, UserProfile profile, UserSecurity security, DateTime createdAt)
    {
        return new User(id, email, profile, security, createdAt);
    }

    public void UpdateProfile(string username, string name, DateOnly dateOfBirth)
    {
        Profile = Profile.UpdateUsername(username).UpdateName(name).UpdateDateOfBirth(dateOfBirth);
    }

    public void UpdateUsername(string newUsername)
    {
        Profile = Profile.UpdateUsername(newUsername);
    }

    public void ChangeEmail(Email newEmail)
    {
        if (newEmail == null)
            throw new ArgumentNullException(nameof(newEmail));

        Email = newEmail;
    }

    public void ChangePassword(string newPassword, IPasswordHasher passwordHasher)
    {
        if (passwordHasher == null)
            throw new ArgumentNullException(nameof(passwordHasher));

        var hashedPassword = passwordHasher.HashPassword(newPassword);
        Security = Security.ChangePassword(hashedPassword);

        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }

    public void RecordLogin()
    {
        Security = Security.RecordLogin();
    }

    public bool IsPasswordValid(string providedPassword, IPasswordHasher passwordHasher)
    {
        if (passwordHasher == null)
            throw new ArgumentNullException(nameof(passwordHasher));

        if (string.IsNullOrWhiteSpace(providedPassword))
            return false;

        return passwordHasher.VerifyPassword(providedPassword, Security.PasswordHash);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
