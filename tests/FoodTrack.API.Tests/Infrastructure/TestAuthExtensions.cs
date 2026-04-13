using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace FoodTrack.API.Tests;

internal static class TestAuthExtensions
{
    public static async Task AuthenticateAsDefaultOperatorAsync(this HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            badgeCode = "OP-1001",
            pin = "1234"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<LoginPayload>();
        payload.Should().NotBeNull();
        payload!.AccessToken.Should().NotBeNullOrWhiteSpace();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", payload.AccessToken);
    }

    private sealed class LoginPayload
    {
        public string AccessToken { get; init; } = string.Empty;
    }
}
