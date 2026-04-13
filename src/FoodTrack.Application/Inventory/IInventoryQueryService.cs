namespace FoodTrack.Application.Inventory;

/// <summary>
/// Exposes inventory-oriented read models to presentation layers.
/// </summary>
public interface IInventoryQueryService
{
    /// <summary>
    /// Returns the expiration dashboard summary for warehouse users.
    /// </summary>
    Task<ExpirationDashboardDto> GetExpirationDashboardAsync(DateTime asOfUtc, CancellationToken cancellationToken);

    /// <summary>
    /// Returns the product catalog ordered for operator browsing.
    /// </summary>
    Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns dispatchable batches in FIFO order for a product.
    /// </summary>
    Task<IReadOnlyList<BatchListItemDto>> GetFifoBatchesAsync(Guid productId, DateTime asOfUtc, CancellationToken cancellationToken);
}
