using FoodTrack.Domain.Enums;

namespace FoodTrack.Application.Management;

/// <summary>
/// Captures the editable product data used during updates.
/// </summary>
public sealed record UpdateProductCommand(
    string Name,
    string Sku,
    ProductCategory Category,
    UnitOfMeasure Unit,
    int MinStockLevel);
