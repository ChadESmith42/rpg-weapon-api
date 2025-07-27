using WeaponApi.Domain.Common;

namespace WeaponApi.Domain.User;

/// <summary>
/// Domain event raised when a user's password is changed.
/// </summary>
public sealed record UserPasswordChangedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string EventType { get; }
    public int Version { get; }

    public UserId UserId { get; }

    public UserPasswordChangedEvent(UserId userId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EventType = nameof(UserPasswordChangedEvent);
        Version = 1;

        UserId = userId;
    }
}
