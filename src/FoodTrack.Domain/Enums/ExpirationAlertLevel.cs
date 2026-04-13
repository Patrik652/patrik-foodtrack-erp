namespace FoodTrack.Domain.Enums;

/// <summary>
/// Highlights how urgently a batch should be handled before it expires.
/// </summary>
public enum ExpirationAlertLevel
{
    None = 0,
    Notice30Days = 30,
    Warning14Days = 14,
    Critical7Days = 7,
    Expired = -1
}
