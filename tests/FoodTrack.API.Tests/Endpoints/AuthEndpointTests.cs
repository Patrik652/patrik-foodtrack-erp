using System.Net.Http.Json;
using FluentAssertions;

namespace FoodTrack.API.Tests.Endpoints;

[Collection(ApiTestCollection.Name)]
public sealed class AuthEndpointTests(FoodTrackApiFactory factory) : IClassFixture<FoodTrackApiFactory>
{
    [Fact]
    public async Task Login_ShouldReturnBearerTokenForSeededOperator()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            badgeCode = "OP-1001",
            pin = "1234"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<LoginResponse>();

        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();
        payload.OperatorName.Should().Be("Roman Skladnik");
    }

    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string OperatorName { get; init; } = string.Empty;
    }
}
