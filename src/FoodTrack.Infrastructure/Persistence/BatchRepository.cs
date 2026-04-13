using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodTrack.Infrastructure.Persistence;

/// <summary>
/// Reads batches through EF Core.
/// </summary>
public sealed class BatchRepository(FoodTrackDbContext dbContext) : IBatchRepository
{
    /// <inheritdoc />
    public async Task AddAsync(Batch batch, CancellationToken cancellationToken)
    {
        await dbContext.Batches.AddAsync(batch, cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteAsync(Batch batch, CancellationToken cancellationToken)
    {
        dbContext.Batches.Remove(batch);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<Batch?> GetByIdAsync(Guid batchId, CancellationToken cancellationToken)
    {
        return await dbContext.Batches
            .SingleOrDefaultAsync(batch => batch.Id == batchId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Batch>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Batches
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Batch>> ListByProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await dbContext.Batches
            .Where(batch => batch.ProductId == productId)
            .ToListAsync(cancellationToken);
    }
}
