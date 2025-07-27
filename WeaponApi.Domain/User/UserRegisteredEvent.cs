using WeaponApi.Domain.Common;

namespace WeaponApi.Domain.User;

/// <summary>
/// Domain event raised when a user is registered.
/// </summary>
public sealed record UserRegisteredEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType { get; }
    public int Version { get; }

    public UserId UserId { get; }
    public Email Email { get; }

    public UserRegisteredEvent(UserId userId, Email email)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EventType = nameof(UserRegisteredEvent);
        Version = 1;

        UserId = userId;
        Email = email;
    }
}
