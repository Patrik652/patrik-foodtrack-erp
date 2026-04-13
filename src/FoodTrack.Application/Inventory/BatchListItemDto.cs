namespace FoodTrack.Application.Inventory;

/// <summary>
/// Provides a read model for warehouse batches.
/// </summary>
public sealed class BatchListItemDto
{
    /// <summary>
    /// Gets the batch identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the lot number.
    /// </summary>
    public string BatchNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the current quantity.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Gets the location code.
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// Gets the expiration date in UTC.
    /// </summary>
    public DateTime ExpirationDate { get; init; }

    /// <summary>
    /// Gets the status label.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the expiration warning label.
    /// </summary>
    public string ExpirationAlert { get; init; } = string.Empty;
}
