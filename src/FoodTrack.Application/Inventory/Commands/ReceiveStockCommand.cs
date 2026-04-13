namespace FoodTrack.Application.Inventory.Commands;

/// <summary>
/// Captures the data required to receive a new warehouse batch.
/// </summary>
public sealed record ReceiveStockCommand(
    Guid ProductId,
    string BatchNumber,
    DateTime ManufactureDate,
    DateTime ExpirationDate,
    decimal Quantity,
    string Location,
    string Note,
    string PerformedBy,
    DateTime ReceivedAtUtc);
