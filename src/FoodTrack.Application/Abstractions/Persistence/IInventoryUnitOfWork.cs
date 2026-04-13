namespace FoodTrack.Application.Abstractions.Persistence;

/// <summary>
/// Commits inventory write operations as one unit.
/// </summary>
public interface IInventoryUnitOfWork
{
    /// <summary>
    /// Persists pending changes.
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
