namespace FoodTrack.Domain.Enums;

/// <summary>
/// Describes how stock changed for a batch.
/// </summary>
public enum StockMovementType
{
    Receive = 1,
    Dispatch = 2,
    Adjust = 3,
    Return = 4
}
