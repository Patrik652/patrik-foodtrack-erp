namespace FoodTrack.Application.Inventory;

/// <summary>
/// Aggregates expiration metrics and urgent batch alerts for the dashboard.
/// </summary>
public sealed class ExpirationDashboardDto
{
    /// <summary>
    /// Gets the dashboard reference timestamp in UTC.
    /// </summary>
    public DateTime AsOfUtc { get; init; }

    /// <summary>
    /// Gets the count of expired batches.
    /// </summary>
    public int ExpiredCount { get; init; }

    /// <summary>
    /// Gets the count of batches expiring within 7 days.
    /// </summary>
    public int Critical7DaysCount { get; init; }

    /// <summary>
    /// Gets the count of batches expiring within 14 days.
    /// </summary>
    public int Warning14DaysCount { get; init; }

    /// <summary>
    /// Gets the count of batches expiring within 30 days.
    /// </summary>
    public int Notice30DaysCount { get; init; }

    /// <summary>
    /// Gets the prioritized alert items shown to operators.
    /// </summary>
    public IReadOnlyList<ExpirationAlertItemDto> Items { get; init; } = [];
}
