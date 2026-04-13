using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodTrack.Infrastructure.Persistence;

/// <summary>
/// Reads products through EF Core.
/// </summary>
public sealed class ProductRepository(FoodTrackDbContext dbContext) : IProductRepository
{
    /// <inheritdoc />
    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await dbContext.Products.AddAsync(product, cancellationToken);
    }

    /// <inheritdoc />
    public Task DeleteAsync(Product product, CancellationToken cancellationToken)
    {
        dbContext.Products.Remove(product);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .SingleOrDefaultAsync(product => product.Id == productId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }
}
