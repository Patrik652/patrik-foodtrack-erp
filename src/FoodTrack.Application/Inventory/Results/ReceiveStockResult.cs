namespace FoodTrack.Application.Inventory.Results;

/// <summary>
/// Describes the result of receiving a new stock batch.
/// </summary>
public sealed class ReceiveStockResult
{
    /// <summary>
    /// Gets the new batch identifier.
    /// </summary>
    public Guid BatchId { get; init; }

    /// <summary>
    /// Gets the receive movement identifier.
    /// </summary>
    public Guid MovementId { get; init; }

    /// <summary>
    /// Gets the normalized batch number.
    /// </summary>
    public string BatchNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the received quantity.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Gets the normalized location code.
    /// </summary>
    public string Location { get; init; } = string.Empty;
}
