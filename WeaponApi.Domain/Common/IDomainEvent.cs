namespace WeaponApi.Domain.Common;

/// <summary>
/// Marker interface for domain events that represent significant business occurrences within the domain.
/// Domain events are immutable facts about something that happened in the business domain.
/// They are used to communicate changes between aggregates and trigger side effects.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this domain event instance.
    /// Useful for event tracking, deduplication, and correlation.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the UTC timestamp when this domain event occurred.
    /// This represents when the business event actually happened, not when it was processed.
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Gets the type name of this domain event.
    /// Used for event routing, serialization, and event store operations.
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Gets the version of this event schema.
    /// Useful for event evolution and backward compatibility scenarios.
    /// </summary>
    int Version { get; }
}
