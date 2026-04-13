using System.Net.Http.Json;
using FluentAssertions;

namespace FoodTrack.API.Tests.Endpoints;

[Collection(ApiTestCollection.Name)]
public sealed class DashboardEndpointTests(FoodTrackApiFactory factory) : IClassFixture<FoodTrackApiFactory>
{
    [Fact]
    public async Task GetExpirationOverview_ShouldReturnSummaryAndPrioritizedItems()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/dashboard/expiration-overview?asOfUtc=2026-04-12T08:00:00Z");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ExpirationOverviewResponse>();
        payload.Should().NotBeNull();
        payload!.Critical7DaysCount.Should().BeGreaterThan(0);
        payload.Items.Should().NotBeEmpty();
        payload.Items[0].Alert.Should().Be("Critical7Days");
    }

    public sealed class ExpirationOverviewResponse
    {
        public DateTime AsOfUtc { get; init; }
        public int ExpiredCount { get; init; }
        public int Critical7DaysCount { get; init; }
        public int Warning14DaysCount { get; init; }
        public int Notice30DaysCount { get; init; }
        public List<ExpirationOverviewItemResponse> Items { get; init; } = [];
    }

    public sealed class ExpirationOverviewItemResponse
    {
        public string ProductName { get; init; } = string.Empty;
        public string BatchNumber { get; init; } = string.Empty;
        public string Alert { get; init; } = string.Empty;
        public int DaysUntilExpiration { get; init; }
    }
}
