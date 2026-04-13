using FluentAssertions;
using FoodTrack.Mobile.Services;
using FoodTrack.Presentation.Tests.Infrastructure;

namespace FoodTrack.Presentation.Tests.Services;

public sealed class ApiServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldPersistBearerTokenInSession()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            FakeHttpMessageHandler.Json("""
                {"accessToken":"jwt-123","badgeCode":"OP-1001","operatorName":"Roman Skladnik"}
                """));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var session = new AuthSessionService();
        var service = new ApiService(httpClient, session);

        var response = await service.LoginAsync("OP-1001", "1234");

        response.OperatorName.Should().Be("Roman Skladnik");
        session.IsAuthenticated.Should().BeTrue();
        session.AccessToken.Should().Be("jwt-123");
    }

    [Fact]
    public async Task GetProductsAsync_ShouldSendBearerTokenWhenSessionExists()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            FakeHttpMessageHandler.Json("""
                [{"id":"11111111-1111-1111-1111-111111111111","name":"Mlieko Tatranske 1L","sku":"MLK-001","category":"Dairy","unit":"Liter","minStockLevel":25}]
                """));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost") };
        var session = new AuthSessionService();
        session.SetSession(new OperatorLoginResponse
        {
            AccessToken = "jwt-abc",
            BadgeCode = "OP-1001",
            OperatorName = "Roman Skladnik"
        });
        var service = new ApiService(httpClient, session);

        var products = await service.GetProductsAsync();

        products.Should().ContainSingle();
        handler.LastAuthorizationHeader.Should().NotBeNull();
        handler.LastAuthorizationHeader!.Scheme.Should().Be("Bearer");
        handler.LastAuthorizationHeader.Parameter.Should().Be("jwt-abc");
    }
}
