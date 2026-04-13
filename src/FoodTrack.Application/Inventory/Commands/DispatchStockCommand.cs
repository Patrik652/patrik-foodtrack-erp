namespace FoodTrack.Application.Inventory.Commands;

/// <summary>
/// Captures the data required to dispatch stock for a product.
/// </summary>
public sealed record DispatchStockCommand(
    Guid ProductId,
    decimal Quantity,
    DateTime DispatchedAtUtc,
    string Note,
    string PerformedBy);
