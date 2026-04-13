namespace FoodTrack.Application.Inventory.Results;

/// <summary>
/// Describes one batch consumed during a dispatch operation.
/// </summary>
public sealed class DispatchStockLineResult
{
    /// <summary>
    /// Gets the batch identifier.
    /// </summary>
    public Guid BatchId { get; init; }

    /// <summary>
    /// Gets the batch number.
    /// </summary>
    public string BatchNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the dispatched quantity from this batch.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Gets the location from which the stock was dispatched.
    /// </summary>
    public string Location { get; init; } = string.Empty;
}
