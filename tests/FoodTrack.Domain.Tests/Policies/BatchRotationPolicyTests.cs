using FluentAssertions;
using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Policies;

namespace FoodTrack.Domain.Tests.Policies;

public sealed class BatchRotationPolicyTests
{
    [Fact]
    public void SortForDispatch_ShouldPreferEarlierExpirationThenOlderManufactureDate()
    {
        var asOfUtc = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
        var productId = Guid.NewGuid();
        var batches = new[]
        {
            Batch.Create(productId, "LOT-003", asOfUtc.AddDays(-2), asOfUtc.AddDays(12), 6m, "A-03"),
            Batch.Create(productId, "LOT-001", asOfUtc.AddDays(-3), asOfUtc.AddDays(7), 6m, "A-01"),
            Batch.Create(productId, "LOT-002", asOfUtc.AddDays(-5), asOfUtc.AddDays(7), 6m, "A-02")
        };

        var ordered = BatchRotationPolicy.SortForDispatch(batches, asOfUtc);

        ordered.Select(batch => batch.BatchNumber).Should().Equal("LOT-002", "LOT-001", "LOT-003");
    }
}
