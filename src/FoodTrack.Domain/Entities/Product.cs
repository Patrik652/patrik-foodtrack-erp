using FoodTrack.Domain.Enums;

namespace FoodTrack.Domain.Entities;

/// <summary>
/// Describes a warehouse product tracked inside FoodTrack ERP.
/// </summary>
public sealed class Product
{
    private Product()
    {
    }

    private Product(Guid id, string name, string sku, ProductCategory category, UnitOfMeasure unit, int minStockLevel)
    {
        Id = id;
        Name = name;
        Sku = sku;
        Category = category;
        Unit = unit;
        MinStockLevel = minStockLevel;
    }

    /// <summary>
    /// Gets the unique identifier of the product.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the display name used by warehouse staff.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the stock keeping unit of the product.
    /// </summary>
    public string Sku { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the product category.
    /// </summary>
    public ProductCategory Category { get; private set; }

    /// <summary>
    /// Gets the tracked stock unit.
    /// </summary>
    public UnitOfMeasure Unit { get; private set; }

    /// <summary>
    /// Gets the stock threshold under which replenishment should be considered.
    /// </summary>
    public int MinStockLevel { get; private set; }

    /// <summary>
    /// Creates a validated product instance.
    /// </summary>
    public static Product Create(string name, string sku, ProductCategory category, UnitOfMeasure unit, int minStockLevel)
    {
        return new Product(
            Guid.NewGuid(),
            NormalizeRequired(name, nameof(name)),
            NormalizeSku(sku),
            category,
            unit,
            ValidateMinStockLevel(minStockLevel));
    }

    /// <summary>
    /// Updates the stock threshold for the product.
    /// </summary>
    public void UpdateMinStockLevel(int minStockLevel)
    {
        MinStockLevel = ValidateMinStockLevel(minStockLevel);
    }

    /// <summary>
    /// Updates the editable catalog data of the product.
    /// </summary>
    public void Update(string name, string sku, ProductCategory category, UnitOfMeasure unit, int minStockLevel)
    {
        Name = NormalizeRequired(name, nameof(name));
        Sku = NormalizeSku(sku);
        Category = category;
        Unit = unit;
        MinStockLevel = ValidateMinStockLevel(minStockLevel);
    }

    private static int ValidateMinStockLevel(int minStockLevel)
    {
        if (minStockLevel < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minStockLevel), "Minimum stock level cannot be negative.");
        }

        return minStockLevel;
    }

    private static string NormalizeSku(string sku)
    {
        return NormalizeRequired(sku, nameof(sku)).ToUpperInvariant();
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", paramName);
        }

        return value.Trim();
    }
}
