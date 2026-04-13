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
