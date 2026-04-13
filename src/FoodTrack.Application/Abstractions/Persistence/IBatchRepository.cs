using FoodTrack.Domain.Entities;

namespace FoodTrack.Application.Abstractions.Persistence;

/// <summary>
/// Reads warehouse batches for inventory queries.
/// </summary>
public interface IBatchRepository
{
    /// <summary>
    /// Adds a new batch to persistence.
    /// </summary>
    Task AddAsync(Batch batch, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes one batch from persistence.
    /// </summary>
    Task DeleteAsync(Batch batch, CancellationToken cancellationToken);

    /// <summary>
    /// Returns one batch by identifier when it exists.
    /// </summary>
    Task<Batch?> GetByIdAsync(Guid batchId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all batches used by dashboard-style queries.
    /// </summary>
    Task<IReadOnlyList<Batch>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns batches for the supplied product.
    /// </summary>
    Task<IReadOnlyList<Batch>> ListByProductAsync(Guid productId, CancellationToken cancellationToken);
}
