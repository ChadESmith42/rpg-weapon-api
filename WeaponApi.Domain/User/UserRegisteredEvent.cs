using WeaponApi.Domain.Common;

namespace WeaponApi.Domain.User;

/// <summary>
/// Domain event raised when a user is registered.
/// </summary>
public sealed record UserRegisteredEvent : IDomainEvent
{
    public UserId UserId { get; }
    public Email Email { get; }
    public DateTime OccurredAt { get; }

    public UserRegisteredEvent(UserId userId, Email email)
    {
        UserId = userId;
        Email = email;
        OccurredAt = DateTime.UtcNow;
    }
}
