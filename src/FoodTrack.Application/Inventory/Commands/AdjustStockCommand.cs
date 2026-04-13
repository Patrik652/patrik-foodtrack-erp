namespace FoodTrack.Application.Inventory.Commands;

/// <summary>
/// Captures the data required to apply a stock-count adjustment to a batch.
/// </summary>
public sealed record AdjustStockCommand(
    Guid BatchId,
    decimal NewQuantity,
    DateTime AdjustedAtUtc,
    string Note,
    string PerformedBy);
