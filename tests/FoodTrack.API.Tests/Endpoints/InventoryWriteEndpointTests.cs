using System.Net.Http.Json;
using FluentAssertions;
using FoodTrack.Domain.Enums;
using FoodTrack.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FoodTrack.API.Tests.Endpoints;

[Collection(ApiTestCollection.Name)]
public sealed class InventoryWriteEndpointTests(FoodTrackApiFactory factory) : IClassFixture<FoodTrackApiFactory>
{
    [Fact]
    public async Task Receive_ShouldRejectAnonymousWarehouseWrites()
    {
        using var client = factory.CreateClient();
        var products = await client.GetFromJsonAsync<List<ProductResponse>>("/api/products");
        var milk = products!.Single(product => product.Name == "Mlieko Tatranske 1L");

        var response = await client.PostAsJsonAsync("/api/inventory/receive", new
        {
            productId = milk.Id,
            batchNumber = "LOT-TEST-ANON-001",
            manufactureDate = "2026-04-12T08:00:00Z",
            expirationDate = "2026-05-12T08:00:00Z",
            quantity = 10m,
            location = "A-11",
            note = "Anonymous receive should fail",
            performedBy = "Anonymous Tester",
            receivedAtUtc = "2026-04-12T08:00:00Z"
        });

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InventoryWriteFlow_ShouldReceiveDispatchAndAdjustStockThroughAuthenticatedOperatorIdentity()
    {
        using var client = factory.CreateClient();
        var products = await client.GetFromJsonAsync<List<ProductResponse>>("/api/products");
        var milk = products!.Single(product => product.Name == "Mlieko Tatranske 1L");
        var accessToken = await LoginAsync(client);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var receiveResponse = await client.PostAsJsonAsync("/api/inventory/receive", new
        {
            productId = milk.Id,
            batchNumber = "LOT-TEST-001",
            manufactureDate = "2026-04-12T08:00:00Z",
            expirationDate = "2026-05-12T08:00:00Z",
            quantity = 40m,
            location = "A-11",
            note = "Test receive",
            receivedAtUtc = "2026-04-12T08:00:00Z"
        });
        receiveResponse.EnsureSuccessStatusCode();
        var receivePayload = await receiveResponse.Content.ReadFromJsonAsync<ReceiveResponse>();

        var dispatchResponse = await client.PostAsJsonAsync("/api/inventory/dispatch", new
        {
            productId = milk.Id,
            quantity = 2m,
            dispatchedAtUtc = "2026-04-12T09:00:00Z",
            note = "Test dispatch"
        });
        dispatchResponse.EnsureSuccessStatusCode();
        var dispatchPayload = await dispatchResponse.Content.ReadFromJsonAsync<DispatchResponse>();

        var adjustResponse = await client.PostAsJsonAsync("/api/inventory/adjustments", new
        {
            batchId = receivePayload!.BatchId,
            newQuantity = 22m,
            adjustedAtUtc = "2026-04-12T10:00:00Z",
            note = "Test adjustment"
        });
        adjustResponse.EnsureSuccessStatusCode();
        var adjustPayload = await adjustResponse.Content.ReadFromJsonAsync<AdjustResponse>();

        var fifoBatches = await client.GetFromJsonAsync<List<FifoBatchResponse>>($"/api/products/{milk.Id}/fifo-batches?asOfUtc=2026-04-12T10:00:00Z");
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FoodTrackDbContext>();
        var receiveMovement = await dbContext.StockMovements.SingleAsync(movement =>
            movement.BatchId == receivePayload.BatchId
            && movement.Type == StockMovementType.Receive);

        receivePayload.Should().NotBeNull();
        receivePayload!.BatchNumber.Should().Be("LOT-TEST-001");
        dispatchPayload.Should().NotBeNull();
        dispatchPayload!.AllocatedQuantity.Should().Be(2m);
        adjustPayload.Should().NotBeNull();
        adjustPayload!.NewQuantity.Should().Be(22m);
        fifoBatches.Should().Contain(batch => batch.BatchNumber == "LOT-TEST-001" && batch.Quantity == 22m);
        receiveMovement.PerformedBy.Should().Be("Roman Skladnik");
    }

    private static async Task<string> LoginAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            badgeCode = "OP-1001",
            pin = "1234"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();
        payload.Should().NotBeNull();

        return payload!.AccessToken;
    }

    public sealed class ProductResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }

    public sealed class ReceiveResponse
    {
        public Guid BatchId { get; init; }
        public string BatchNumber { get; init; } = string.Empty;
    }

    public sealed class DispatchResponse
    {
        public decimal AllocatedQuantity { get; init; }
    }

    public sealed class AdjustResponse
    {
        public decimal NewQuantity { get; init; }
    }

    public sealed class FifoBatchResponse
    {
        public string BatchNumber { get; init; } = string.Empty;
        public decimal Quantity { get; init; }
    }

    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = string.Empty;
    }
}
