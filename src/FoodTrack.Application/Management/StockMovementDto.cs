namespace FoodTrack.Application.Management;

/// <summary>
/// Provides a stock movement read model for detail and history views.
/// </summary>
public sealed class StockMovementDto
{
    /// <summary>
    /// Gets the movement identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the related batch identifier.
    /// </summary>
    public Guid BatchId { get; init; }

    /// <summary>
    /// Gets the movement type.
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Gets the movement quantity.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Gets the timestamp in UTC.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Gets the operator note.
    /// </summary>
    public string Note { get; init; } = string.Empty;

    /// <summary>
    /// Gets the operator identity.
    /// </summary>
    public string PerformedBy { get; init; } = string.Empty;
}
