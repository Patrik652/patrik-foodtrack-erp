namespace FoodTrack.Domain.Enums;

/// <summary>
/// Represents the lifecycle state of an inventory batch.
/// </summary>
public enum BatchStatus
{
    Active = 1,
    Expired = 2,
    Recalled = 3,
    Depleted = 4
}
