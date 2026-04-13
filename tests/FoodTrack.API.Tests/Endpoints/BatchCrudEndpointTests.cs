using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using FoodTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTrack.API.Tests.Endpoints;

[Collection(ApiTestCollection.Name)]
public sealed class BatchCrudEndpointTests(FoodTrackApiFactory factory) : IClassFixture<FoodTrackApiFactory>
{
    [Fact]
    public async Task GetBatchById_ShouldReturnBatchWithMovementHistory()
    {
        using var client = factory.CreateClient();
        var batchId = await GetSeedBatchIdAsync();

        var response = await client.GetAsync($"/api/batches/{batchId}");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<BatchDetailResponse>();
        payload.Should().NotBeNull();
        payload!.Movements.Should().NotBeEmpty();
        payload.ProductName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task UpdateBatch_ShouldChangeLocationQuantityAndStatus()
    {
        using var client = factory.CreateClient();
        await client.AuthenticateAsDefaultOperatorAsync();
        var batchId = await GetSeedBatchIdAsync();

        var response = await client.PutAsJsonAsync($"/api/batches/{batchId}", new
        {
            location = "Z-99",
            quantity = 77m,
            status = "Recalled",
            updatedAtUtc = "2026-04-12T12:00:00Z"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<BatchDetailResponse>();

        payload.Should().NotBeNull();
        payload!.Location.Should().Be("Z-99");
        payload.Quantity.Should().Be(77m);
        payload.Status.Should().Be("Recalled");
    }

    [Fact]
    public async Task DeleteBatch_ShouldRemoveBatch()
    {
        using var client = factory.CreateClient();
        await client.AuthenticateAsDefaultOperatorAsync();
        var milk = await GetMilkProductAsync(client);

        var receiveResponse = await client.PostAsJsonAsync("/api/inventory/receive", new
        {
            productId = milk.Id,
            batchNumber = "LOT-BATCH-DELETE-001",
            manufactureDate = "2026-04-12T08:00:00Z",
            expirationDate = "2026-05-12T08:00:00Z",
            quantity = 14m,
            location = "X-01",
            note = "Delete me",
            receivedAtUtc = "2026-04-12T08:00:00Z"
        });
        receiveResponse.EnsureSuccessStatusCode();
        var created = await receiveResponse.Content.ReadFromJsonAsync<ReceiveResponse>();

        var deleteResponse = await client.DeleteAsync($"/api/batches/{created!.BatchId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getDeletedResponse = await client.GetAsync($"/api/batches/{created.BatchId}");
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RecallBatch_ShouldMarkBatchAsRecalledAndAppendRecallMovement()
    {
        using var client = factory.CreateClient();
        await client.AuthenticateAsDefaultOperatorAsync();
        var milk = await GetMilkProductAsync(client);

        var receiveResponse = await client.PostAsJsonAsync("/api/inventory/receive", new
        {
            productId = milk.Id,
            batchNumber = $"LOT-RECALL-{Guid.NewGuid():N}"[..20].ToUpperInvariant(),
            manufactureDate = "2026-04-12T08:00:00Z",
            expirationDate = "2026-05-12T08:00:00Z",
            quantity = 9m,
            location = "R-01",
            note = "Recall candidate",
            receivedAtUtc = "2026-04-12T08:00:00Z"
        });
        receiveResponse.EnsureSuccessStatusCode();
        var created = await receiveResponse.Content.ReadFromJsonAsync<ReceiveResponse>();

        var recallResponse = await client.PostAsync($"/api/batches/{created!.BatchId}/recall", content: null);

        recallResponse.EnsureSuccessStatusCode();
        var payload = await recallResponse.Content.ReadFromJsonAsync<BatchDetailResponse>();
        payload.Should().NotBeNull();
        payload!.Status.Should().Be("Recalled");
        payload.Movements.Should().Contain(movement =>
            movement.Type == "Recall"
            && movement.Quantity == 9m
            && movement.PerformedBy == "Roman Skladnik");
    }

    private async Task<Guid> GetSeedBatchIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FoodTrackDbContext>();
        return await dbContext.Batches.OrderBy(batch => batch.ExpirationDate).Select(batch => batch.Id).FirstAsync();
    }

    private static async Task<ProductResponse> GetMilkProductAsync(HttpClient client)
    {
        var products = await client.GetFromJsonAsync<List<ProductResponse>>("/api/products");
        return products!.Single(product => product.Name == "Mlieko Tatranske 1L");
    }

    public sealed class BatchDetailResponse
    {
        public Guid Id { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public string BatchNumber { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
        public string Location { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public List<StockMovementResponse> Movements { get; init; } = [];
    }

    public sealed class StockMovementResponse
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
        public string PerformedBy { get; init; } = string.Empty;
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
}
