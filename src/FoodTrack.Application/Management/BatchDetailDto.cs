namespace FoodTrack.Application.Management;

/// <summary>
/// Provides a detailed warehouse batch view including movement history.
/// </summary>
public sealed class BatchDetailDto
{
    /// <summary>
    /// Gets the batch identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the owning product identifier.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the product display name.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the product SKU code.
    /// </summary>
    public string ProductSku { get; init; } = string.Empty;

    /// <summary>
    /// Gets the lot number.
    /// </summary>
    public string BatchNumber { get; init; } = string.Empty;

    /// <summary>
    /// Gets the manufacture date in UTC.
    /// </summary>
    public DateTime ManufactureDate { get; init; }

    /// <summary>
    /// Gets the expiration date in UTC.
    /// </summary>
    public DateTime ExpirationDate { get; init; }

    /// <summary>
    /// Gets the available quantity.
    /// </summary>
    public decimal Quantity { get; init; }

    /// <summary>
    /// Gets the warehouse location.
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// Gets the lifecycle status.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets the expiration alert label.
    /// </summary>
    public string ExpirationAlert { get; init; } = string.Empty;

    /// <summary>
    /// Gets the signed remaining days until expiration.
    /// </summary>
    public int DaysUntilExpiration { get; init; }

    /// <summary>
    /// Gets the movement history.
    /// </summary>
    public IReadOnlyList<StockMovementDto> Movements { get; init; } = [];
}
