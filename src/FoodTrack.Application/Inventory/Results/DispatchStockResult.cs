namespace FoodTrack.Application.Inventory.Results;

/// <summary>
/// Describes the result of a FIFO stock dispatch.
/// </summary>
public sealed class DispatchStockResult
{
    /// <summary>
    /// Gets the affected product identifier.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the requested quantity.
    /// </summary>
    public decimal RequestedQuantity { get; init; }

    /// <summary>
    /// Gets the successfully allocated quantity.
    /// </summary>
    public decimal AllocatedQuantity { get; init; }

    /// <summary>
    /// Gets the batches consumed during the dispatch.
    /// </summary>
    public IReadOnlyList<DispatchStockLineResult> Lines { get; init; } = [];
}
