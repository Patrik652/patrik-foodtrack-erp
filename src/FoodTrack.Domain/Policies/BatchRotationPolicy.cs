using FoodTrack.Domain.Entities;

namespace FoodTrack.Domain.Policies;

/// <summary>
/// Applies FIFO dispatch ordering while respecting expiration constraints.
/// </summary>
public static class BatchRotationPolicy
{
    /// <summary>
    /// Sorts dispatchable batches using expiration-first FIFO rules.
    /// </summary>
    public static IReadOnlyList<Batch> SortForDispatch(IEnumerable<Batch> batches, DateTime asOfUtc)
    {
        ArgumentNullException.ThrowIfNull(batches);

        return batches
            .Where(batch => batch.IsDispatchable(asOfUtc))
            .OrderBy(batch => batch.ExpirationDate)
            .ThenBy(batch => batch.ManufactureDate)
            .ThenBy(batch => batch.BatchNumber, StringComparer.Ordinal)
            .ToArray();
    }
}
