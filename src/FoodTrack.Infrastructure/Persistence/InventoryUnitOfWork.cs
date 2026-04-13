using FoodTrack.Application.Abstractions.Persistence;

namespace FoodTrack.Infrastructure.Persistence;

/// <summary>
/// Commits pending inventory writes through the shared EF Core context.
/// </summary>
public sealed class InventoryUnitOfWork(FoodTrackDbContext dbContext) : IInventoryUnitOfWork
{
    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
