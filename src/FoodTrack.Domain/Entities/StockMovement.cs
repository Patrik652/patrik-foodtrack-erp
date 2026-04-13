using FoodTrack.Domain.Enums;

namespace FoodTrack.Domain.Entities;

/// <summary>
/// Captures an inventory movement that affected a specific batch.
/// </summary>
public sealed class StockMovement
{
    private StockMovement()
    {
    }

    private StockMovement(
        Guid id,
        Guid batchId,
        StockMovementType type,
        decimal quantity,
        DateTime timestamp,
        string note,
        string performedBy)
    {
        Id = id;
        BatchId = batchId;
        Type = type;
        Quantity = quantity;
        Timestamp = timestamp;
        Note = note;
        PerformedBy = performedBy;
    }

    /// <summary>
    /// Gets the unique identifier of the stock movement.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the related batch identifier.
    /// </summary>
    public Guid BatchId { get; private set; }

    /// <summary>
    /// Gets the type of stock movement.
    /// </summary>
    public StockMovementType Type { get; private set; }

    /// <summary>
    /// Gets the moved quantity.
    /// </summary>
    public decimal Quantity { get; private set; }

    /// <summary>
    /// Gets the timestamp stored in UTC.
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Gets the warehouse note entered for the movement.
    /// </summary>
    public string Note { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the user who performed the movement.
    /// </summary>
    public string PerformedBy { get; private set; } = string.Empty;

    /// <summary>
    /// Creates a validated stock movement.
    /// </summary>
    public static StockMovement Create(
        Guid batchId,
        StockMovementType type,
        decimal quantity,
        DateTime timestamp,
        string note,
        string performedBy)
    {
        if (batchId == Guid.Empty)
        {
            throw new ArgumentException("Batch identifier is required.", nameof(batchId));
        }

        if (quantity <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Movement quantity must be positive.");
        }

        return new StockMovement(
            Guid.NewGuid(),
            batchId,
            type,
            quantity,
            timestamp.Kind == DateTimeKind.Utc ? timestamp : timestamp.ToUniversalTime(),
            string.IsNullOrWhiteSpace(note) ? string.Empty : note.Trim(),
            string.IsNullOrWhiteSpace(performedBy) ? throw new ArgumentException("Performed by is required.", nameof(performedBy)) : performedBy.Trim());
    }
}
