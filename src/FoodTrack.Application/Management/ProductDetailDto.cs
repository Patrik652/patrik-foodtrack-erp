namespace FoodTrack.Application.Management;

/// <summary>
/// Provides a detailed view of one tracked product.
/// </summary>
public sealed class ProductDetailDto
{
    /// <summary>
    /// Gets the product identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the SKU code.
    /// </summary>
    public string Sku { get; init; } = string.Empty;

    /// <summary>
    /// Gets the category label.
    /// </summary>
    public string Category { get; init; } = string.Empty;

    /// <summary>
    /// Gets the unit label.
    /// </summary>
    public string Unit { get; init; } = string.Empty;

    /// <summary>
    /// Gets the minimum stock threshold.
    /// </summary>
    public int MinStockLevel { get; init; }

    /// <summary>
    /// Gets the current total stock across active batches.
    /// </summary>
    public decimal CurrentStockQuantity { get; init; }
}
