namespace FoodTrack.Application.Inventory;

/// <summary>
/// Provides a flat product view for list endpoints.
/// </summary>
public sealed class ProductListItemDto
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
    /// Gets the stock keeping unit.
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
}
