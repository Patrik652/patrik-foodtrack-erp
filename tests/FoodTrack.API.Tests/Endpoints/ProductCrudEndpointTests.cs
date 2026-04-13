using System.Net;
using System.Net.Http.Json;
using FluentAssertions;

namespace FoodTrack.API.Tests.Endpoints;

[Collection(ApiTestCollection.Name)]
public sealed class ProductCrudEndpointTests(FoodTrackApiFactory factory) : IClassFixture<FoodTrackApiFactory>
{
    [Fact]
    public async Task GetProductById_ShouldReturnTrackedProduct()
    {
        using var client = factory.CreateClient();
        var products = await client.GetFromJsonAsync<List<ProductResponse>>("/api/products");
        var milk = products!.Single(product => product.Name == "Mlieko Tatranske 1L");

        var response = await client.GetAsync($"/api/products/{milk.Id}");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ProductResponse>();
        payload.Should().NotBeNull();
        payload!.Sku.Should().Be("MLK-001");
    }

    [Fact]
    public async Task ProductCrudFlow_ShouldCreateUpdateAndDeleteProduct()
    {
        using var client = factory.CreateClient();
        await client.AuthenticateAsDefaultOperatorAsync();

        var createResponse = await client.PostAsJsonAsync("/api/products", new
        {
            name = "Jogurt Biely 150g",
            sku = "MLK-010",
            category = "Dairy",
            unit = "Piece",
            minStockLevel = 24
        });

        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var updateResponse = await client.PutAsJsonAsync($"/api/products/{created!.Id}", new
        {
            name = "Jogurt Biely Smotanovy 150g",
            sku = "MLK-010A",
            category = "Dairy",
            unit = "Piece",
            minStockLevel = 30
        });

        updateResponse.EnsureSuccessStatusCode();
        var updated = await updateResponse.Content.ReadFromJsonAsync<ProductResponse>();

        var deleteResponse = await client.DeleteAsync($"/api/products/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getDeletedResponse = await client.GetAsync($"/api/products/{created.Id}");

        updated.Should().NotBeNull();
        updated!.Name.Should().Be("Jogurt Biely Smotanovy 150g");
        updated.Sku.Should().Be("MLK-010A");
        updated.MinStockLevel.Should().Be(30);
        getDeletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    public sealed class ProductResponse
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Sku { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string Unit { get; init; } = string.Empty;
        public int MinStockLevel { get; init; }
    }
}
