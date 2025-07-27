namespace WeaponApi.Application.Common;

/// <summary>
/// Generic repository interface for aggregate root persistence operations.
/// Follows DDD principles by providing business-oriented methods rather than CRUD operations.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type</typeparam>
/// <typeparam name="TId">The strongly-typed identifier for the aggregate</typeparam>
public interface IRepository<TAggregate, in TId>
    where TAggregate : class
{
    /// <summary>
    /// Retrieves an aggregate by its unique identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The aggregate if found, null otherwise</returns>
    Task<TAggregate?> FindByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new aggregate to the repository.
    /// </summary>
    /// <param name="aggregate">The aggregate to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing aggregate in the repository.
    /// </summary>
    /// <param name="aggregate">The aggregate to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an aggregate from the repository.
    /// </summary>
    /// <param name="aggregate">The aggregate to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an aggregate with the specified identifier exists.
    /// </summary>
    /// <param name="id">The aggregate identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the aggregate exists, false otherwise</returns>
    Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
}
