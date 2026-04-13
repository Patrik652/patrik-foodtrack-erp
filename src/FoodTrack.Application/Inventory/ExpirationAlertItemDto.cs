namespace FoodTrack.Application.Inventory;

/// <summary>
/// Represents one prioritized batch alert in the expiration dashboard.
/// </summary>
public sealed class ExpirationAlertItemDto
{
    /// <summary>
    /// Gets the related product identifier.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the related batch identifier.
    /// </summary>
    public Guid BatchId { get; init; }

    /// <summary>
    /// Gets the product display name.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the product SKU.
    /// </summary>
    public string ProductSku { get; init; } = string.Empty;

    /// <summary>
    /// Gets the batch number.
    /// </summary>
    public string BatchNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the warehouse location code.
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// Gets the current quantity.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Gets the batch expiration date in UTC.
    /// </summary>
    public DateTime ExpirationDate { get; init; }

    /// <summary>
    /// Gets the whole-day delta between the dashboard timestamp and expiration date.
    /// </summary>
    public int DaysUntilExpiration { get; init; }

    /// <summary>
    /// Gets the alert bucket label.
    /// </summary>
    public string Alert { get; init; } = string.Empty;
}
