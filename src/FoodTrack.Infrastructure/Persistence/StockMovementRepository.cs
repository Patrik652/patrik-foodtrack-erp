using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodTrack.Infrastructure.Persistence;

/// <summary>
/// Persists stock movements through EF Core.
/// </summary>
public sealed class StockMovementRepository(FoodTrackDbContext dbContext) : IStockMovementRepository
{
    /// <inheritdoc />
    public async Task AddAsync(StockMovement movement, CancellationToken cancellationToken)
    {
        await dbContext.StockMovements.AddAsync(movement, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(IEnumerable<StockMovement> movements, CancellationToken cancellationToken)
    {
        await dbContext.StockMovements.AddRangeAsync(movements, cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteByBatchIdAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var movements = dbContext.StockMovements.Where(movement => movement.BatchId == batchId);
        dbContext.StockMovements.RemoveRange(movements);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<StockMovement?> GetByIdAsync(Guid movementId, CancellationToken cancellationToken)
    {
        return await dbContext.StockMovements
            .AsNoTracking()
            .SingleOrDefaultAsync(movement => movement.Id == movementId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StockMovement>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.StockMovements
            .AsNoTracking()
            .OrderByDescending(movement => movement.Timestamp)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StockMovement>> ListByBatchAsync(Guid batchId, CancellationToken cancellationToken)
    {
        return await dbContext.StockMovements
            .AsNoTracking()
            .Where(movement => movement.BatchId == batchId)
            .OrderByDescending(movement => movement.Timestamp)
            .ToListAsync(cancellationToken);
    }
}
