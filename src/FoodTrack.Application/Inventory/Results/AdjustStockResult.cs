namespace FoodTrack.Application.Inventory.Results;

/// <summary>
/// Describes the result of a stock adjustment.
/// </summary>
public sealed class AdjustStockResult
{
    /// <summary>
    /// Gets the adjusted batch identifier.
    /// </summary>
    public Guid BatchId { get; init; }

    /// <summary>
    /// Gets the adjustment movement identifier.
    /// </summary>
    public Guid MovementId { get; init; }

    /// <summary>
    /// Gets the quantity before the adjustment.
    /// </summary>
    public decimal PreviousQuantity { get; init; }

    /// <summary>
    /// Gets the quantity after the adjustment.
    /// </summary>
    public decimal NewQuantity { get; init; }

    /// <summary>
    /// Gets the resulting batch status.
    /// </summary>
    public string Status { get; init; } = string.Empty;
}
