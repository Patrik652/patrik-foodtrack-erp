using FoodTrack.Domain.Enums;

namespace FoodTrack.Application.Management;

/// <summary>
/// Exposes product, batch, and stock-movement management workflows.
/// </summary>
public interface IWarehouseManagementService
{
    /// <summary>
    /// Returns one product detail view when it exists.
    /// </summary>
    Task<ProductDetailDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken);

    /// <summary>
    /// Creates one new catalog product.
    /// </summary>
    Task<ProductDetailDto> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Updates one existing catalog product.
    /// </summary>
    Task<ProductDetailDto?> UpdateProductAsync(Guid productId, UpdateProductCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes one catalog product when allowed.
    /// </summary>
    Task<bool> DeleteProductAsync(Guid productId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns one batch detail view when it exists.
    /// </summary>
    Task<BatchDetailDto?> GetBatchByIdAsync(Guid batchId, DateTime asOfUtc, CancellationToken cancellationToken);

    /// <summary>
    /// Updates one batch state.
    /// </summary>
    Task<BatchDetailDto?> UpdateBatchAsync(Guid batchId, UpdateBatchCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes one batch and its movement history.
    /// </summary>
    Task<bool> DeleteBatchAsync(Guid batchId, CancellationToken cancellationToken);

    /// <summary>
    /// Marks one batch as recalled and appends an audit movement.
    /// </summary>
    Task<BatchDetailDto?> RecallBatchAsync(Guid batchId, string performedBy, DateTime recalledAtUtc, CancellationToken cancellationToken);

    /// <summary>
    /// Lists stock movements, optionally filtered by batch.
    /// </summary>
    Task<IReadOnlyList<StockMovementDto>> GetStockMovementsAsync(Guid? batchId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns one stock movement detail view when it exists.
    /// </summary>
    Task<StockMovementDto?> GetStockMovementByIdAsync(Guid movementId, CancellationToken cancellationToken);
}
