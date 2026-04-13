using FoodTrack.Domain.Enums;

namespace FoodTrack.Application.Management;

/// <summary>
/// Captures the data required to create a catalog product.
/// </summary>
public sealed record CreateProductCommand(
    string Name,
    string Sku,
    ProductCategory Category,
    UnitOfMeasure Unit,
    int MinStockLevel);
