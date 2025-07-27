using WeaponApi.Domain.Common;

namespace WeaponApi.Domain.User;

/// <summary>
/// Domain event raised when a user's password is changed.
/// </summary>
public sealed record UserPasswordChangedEvent : IDomainEvent
{
    public UserId UserId { get; }
    public DateTime OccurredAt { get; }

    public UserPasswordChangedEvent(UserId userId)
    {
        UserId = userId;
        OccurredAt = DateTime.UtcNow;
    }
}
