using FoodTrack.Domain.Entities;

namespace FoodTrack.Application.Abstractions.Persistence;

/// <summary>
/// Reads product records for application queries.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Adds one new product.
    /// </summary>
    Task AddAsync(Product product, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes one tracked product.
    /// </summary>
    Task DeleteAsync(Product product, CancellationToken cancellationToken);

    /// <summary>
    /// Returns one tracked product by identifier when it exists.
    /// </summary>
    Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all tracked products.
    /// </summary>
    Task<IReadOnlyList<Product>> ListAsync(CancellationToken cancellationToken);
}
