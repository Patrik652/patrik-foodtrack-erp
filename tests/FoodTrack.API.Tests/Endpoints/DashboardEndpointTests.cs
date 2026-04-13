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

    [Fact]
    public async Task GetStockAlerts_ShouldReturnProductsBelowConfiguredMinimumStock()
    {
        using var client = factory.CreateClient();
        await client.AuthenticateAsDefaultOperatorAsync();

        var sku = $"LOW-{Guid.NewGuid():N}"[..12].ToUpperInvariant();
        var createProductResponse = await client.PostAsJsonAsync("/api/products", new
        {
            name = "Tvaroh Jemny 250g",
            sku,
            category = "Dairy",
            unit = "Piece",
            minStockLevel = 40
        });
        createProductResponse.EnsureSuccessStatusCode();
        var createdProduct = await createProductResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var receiveResponse = await client.PostAsJsonAsync("/api/inventory/receive", new
        {
            productId = createdProduct!.Id,
            batchNumber = $"LOT-{Guid.NewGuid():N}"[..16].ToUpperInvariant(),
            manufactureDate = "2026-04-12T08:00:00Z",
            expirationDate = "2026-05-12T08:00:00Z",
            quantity = 7m,
            location = "LOW-01",
            note = "Nizky stav skladu",
            receivedAtUtc = "2026-04-12T08:00:00Z"
        });
        receiveResponse.EnsureSuccessStatusCode();

        var response = await client.GetAsync("/api/dashboard/stock-alerts?asOfUtc=2026-04-12T08:00:00Z");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<List<LowStockAlertResponse>>();
        payload.Should().NotBeNull();
        var alerts = payload!;
        alerts.Should().ContainSingle(item => item.ProductId == createdProduct.Id);
        alerts.Single(item => item.ProductId == createdProduct.Id).ShortageQuantity.Should().Be(33m);
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

    public sealed class LowStockAlertResponse
    {
        public Guid ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public decimal CurrentQuantity { get; init; }
        public int MinStockLevel { get; init; }
        public decimal ShortageQuantity { get; init; }
    }

    public sealed class ProductResponse
    {
        public Guid Id { get; init; }
    }
}
