namespace WeaponApi.Domain.Common;

/// <summary>
/// Base abstract class for domain events that provides common implementation
/// of the IDomainEvent interface properties.
/// Inheriting from this class ensures consistent event metadata handling.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <inheritdoc />
    public Guid EventId { get; }

    /// <inheritdoc />
    public DateTime OccurredAt { get; }

    /// <inheritdoc />
    public string EventType { get; }

    /// <inheritdoc />
    public virtual int Version => 1;

    /// <summary>
    /// Initializes a new instance of the DomainEvent class.
    /// Automatically sets EventId, OccurredAt, and EventType.
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        EventType = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the DomainEvent class with a specific occurrence time.
    /// Useful for testing or when recreating events from event store.
    /// </summary>
    /// <param name="occurredAt">The UTC timestamp when the event occurred.</param>
    protected DomainEvent(DateTime occurredAt)
    {
        EventId = Guid.NewGuid();
        OccurredAt = occurredAt;
        EventType = GetType().Name;
    }

    /// <summary>
    /// Initializes a new instance of the DomainEvent class with specific metadata.
    /// Useful when deserializing events from event store.
    /// </summary>
    /// <param name="eventId">The unique identifier for this event.</param>
    /// <param name="occurredAt">The UTC timestamp when the event occurred.</param>
    protected DomainEvent(Guid eventId, DateTime occurredAt)
    {
        EventId = eventId;
        OccurredAt = occurredAt;
        EventType = GetType().Name;
    }
}
