using FluentAssertions;
using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;

namespace FoodTrack.Domain.Tests.Entities;

public sealed class BatchTests
{
    [Fact]
    public void RefreshStatus_ShouldMarkBatchExpired_WhenExpirationDateIsPast()
    {
        var asOfUtc = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
        var batch = Batch.Create(
            Guid.NewGuid(),
            "LOT-2026-04-001",
            asOfUtc.AddDays(-14),
            asOfUtc.AddDays(-1),
            18.5m,
            "A-01");

        batch.RefreshStatus(asOfUtc);

        batch.Status.Should().Be(BatchStatus.Expired);
        batch.GetExpirationAlert(asOfUtc).Should().Be(ExpirationAlertLevel.Expired);
    }

    [Fact]
    public void GetExpirationAlert_ShouldReturnCritical7Days_WhenBatchExpiresSoon()
    {
        var asOfUtc = new DateTime(2026, 4, 12, 8, 0, 0, DateTimeKind.Utc);
        var batch = Batch.Create(
            Guid.NewGuid(),
            "LOT-2026-04-002",
            asOfUtc.AddDays(-2),
            asOfUtc.AddDays(6),
            9m,
            "A-03");

        var alert = batch.GetExpirationAlert(asOfUtc);

        alert.Should().Be(ExpirationAlertLevel.Critical7Days);
    }
}
