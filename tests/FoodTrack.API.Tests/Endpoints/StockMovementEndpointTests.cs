using System.Net.Http.Json;
using FluentAssertions;
using FoodTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTrack.API.Tests.Endpoints;

[Collection(ApiTestCollection.Name)]
public sealed class StockMovementEndpointTests(FoodTrackApiFactory factory) : IClassFixture<FoodTrackApiFactory>
{
    [Fact]
    public async Task ListStockMovements_ShouldReturnBatchHistory()
    {
        using var client = factory.CreateClient();
        var batchId = await GetSeedBatchIdAsync();

        var response = await client.GetAsync($"/api/stock-movements?batchId={batchId}");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<List<StockMovementResponse>>();
        payload.Should().NotBeNull();
        payload!.Should().NotBeEmpty();
        payload.Should().OnlyContain(movement => movement.BatchId == batchId);
    }

    [Fact]
    public async Task GetStockMovementById_ShouldReturnMovementDetail()
    {
        using var client = factory.CreateClient();
        var movementId = await GetSeedMovementIdAsync();

        var response = await client.GetAsync($"/api/stock-movements/{movementId}");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<StockMovementResponse>();
        payload.Should().NotBeNull();
        payload!.Id.Should().Be(movementId);
        payload.PerformedBy.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task ListStockMovements_ShouldOnlyReturnMovementsForRequestedBatch()
    {
        using var client = factory.CreateClient();
        await client.AuthenticateAsDefaultOperatorAsync();
        var products = await client.GetFromJsonAsync<List<ProductResponse>>("/api/products");
        var milk = products!.Single(product => product.Name == "Mlieko Tatranske 1L");

        var trackedBatch = await ReceiveBatchAsync(client, milk.Id, "AUDIT-A");
        _ = await ReceiveBatchAsync(client, milk.Id, "AUDIT-B");

        var response = await client.GetAsync($"/api/stock-movements?batchId={trackedBatch}");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<List<StockMovementResponse>>();
        payload.Should().NotBeNull();
        var movements = payload!;
        movements.Should().ContainSingle();
        movements[0].BatchId.Should().Be(trackedBatch);
        movements[0].Type.Should().Be("Receive");
    }

    private async Task<Guid> GetSeedBatchIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FoodTrackDbContext>();
        return await dbContext.Batches.OrderBy(batch => batch.ExpirationDate).Select(batch => batch.Id).FirstAsync();
    }

    private async Task<Guid> GetSeedMovementIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FoodTrackDbContext>();
        return await dbContext.StockMovements.OrderBy(movement => movement.Timestamp).Select(movement => movement.Id).FirstAsync();
    }

    private static async Task<Guid> ReceiveBatchAsync(HttpClient client, Guid productId, string suffix)
    {
        var response = await client.PostAsJsonAsync("/api/inventory/receive", new
        {
            productId,
            batchNumber = $"{suffix}-{Guid.NewGuid():N}"[..18].ToUpperInvariant(),
            manufactureDate = "2026-04-12T08:00:00Z",
            expirationDate = "2026-05-12T08:00:00Z",
            quantity = 4m,
            location = "AUD-01",
            note = "Audit filter seed",
            receivedAtUtc = "2026-04-12T08:00:00Z"
        });
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ReceiveResponse>();
        return payload!.BatchId;
    }

    public sealed class ProductResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    public sealed class ReceiveResponse
    {
        public Guid BatchId { get; init; }
    }

    public sealed class StockMovementResponse
    {
        public Guid Id { get; init; }
        public Guid BatchId { get; init; }
        public string Type { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
        public string Note { get; init; } = string.Empty;
        public string PerformedBy { get; init; } = string.Empty;
    }
}
