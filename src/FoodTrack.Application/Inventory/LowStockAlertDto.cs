namespace FoodTrack.Application.Inventory;

/// <summary>
/// Describes one product whose effective active stock is below the configured minimum.
/// </summary>
public sealed class LowStockAlertDto
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the warehouse-facing product name.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the SKU code.
    /// </summary>
    public string ProductSku { get; init; } = string.Empty;

    /// <summary>
    /// Gets the product category label.
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unit label.
    /// </summary>
    public string Unit { get; init; } = string.Empty;

    /// <summary>
    /// Gets the effective active quantity available for dispatch.
    /// </summary>
    public decimal CurrentQuantity { get; init; }

    /// <summary>
    /// Gets the configured minimum stock level.
    /// </summary>
    public int MinStockLevel { get; init; }

    /// <summary>
    /// Gets how much stock is missing relative to the configured minimum.
    /// </summary>
    public decimal ShortageQuantity { get; init; }
}
