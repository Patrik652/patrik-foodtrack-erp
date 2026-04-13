using FoodTrack.Domain.Entities;

namespace FoodTrack.Application.Abstractions.Persistence;

/// <summary>
/// Persists stock movement history.
/// </summary>
public interface IStockMovementRepository
{
    /// <summary>
    /// Adds one stock movement.
    /// </summary>
    Task AddAsync(StockMovement movement, CancellationToken cancellationToken);

    /// <summary>
    /// Adds multiple stock movements.
    /// </summary>
    Task AddRangeAsync(IEnumerable<StockMovement> movements, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes all stock movements for the supplied batch.
    /// </summary>
    Task DeleteByBatchIdAsync(Guid batchId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns one movement by identifier when it exists.
    /// </summary>
    Task<StockMovement?> GetByIdAsync(Guid movementId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all persisted stock movements.
    /// </summary>
    Task<IReadOnlyList<StockMovement>> ListAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns all persisted stock movements for one batch.
    /// </summary>
    Task<IReadOnlyList<StockMovement>> ListByBatchAsync(Guid batchId, CancellationToken cancellationToken);
}
