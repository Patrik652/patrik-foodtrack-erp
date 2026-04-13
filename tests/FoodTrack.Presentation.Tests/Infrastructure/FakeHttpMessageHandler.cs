using System.Net;
using System.Net.Http.Headers;

namespace FoodTrack.Presentation.Tests.Infrastructure;

internal sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) : HttpMessageHandler
{
    public AuthenticationHeaderValue? LastAuthorizationHeader { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastAuthorizationHeader = request.Headers.Authorization;
        return Task.FromResult(handler(request));
    }

    public static HttpResponseMessage Json(string payload, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json")
        };
    }
}
