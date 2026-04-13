using System.Net.Http.Json;

namespace FoodTrack.Mobile.Services;

public sealed class ApiService(HttpClient httpClient, AuthSessionService authSessionService) : IApiService
{
    public async Task<OperatorLoginResponse> LoginAsync(string badgeCode, string pin, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("/api/auth/login", new
        {
            badgeCode,
            pin
        }, cancellationToken);

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<OperatorLoginResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("Login response payload was empty.");

        authSessionService.SetSession(payload);
        return payload;
    }

    public Task<IReadOnlyList<ProductSummary>> GetProductsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<ProductSummary>>("/api/products", cancellationToken);

    public Task<ProductDetailModel> GetProductAsync(Guid productId, CancellationToken cancellationToken = default) =>
        GetAsync<ProductDetailModel>($"/api/products/{productId}", cancellationToken);

    public Task<BatchDetailModel> GetBatchAsync(Guid batchId, CancellationToken cancellationToken = default) =>
        GetAsync<BatchDetailModel>($"/api/batches/{batchId}", cancellationToken);

    public Task<IReadOnlyList<BatchSummary>> GetFifoBatchesAsync(Guid productId, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<BatchSummary>>($"/api/products/{productId}/fifo-batches", cancellationToken);

    public Task<DashboardOverview> GetDashboardAsync(DateTime? asOfUtc = null, CancellationToken cancellationToken = default)
    {
        var query = asOfUtc.HasValue ? $"?asOfUtc={Uri.EscapeDataString(asOfUtc.Value.ToString("O"))}" : string.Empty;
        return GetAsync<DashboardOverview>($"/api/dashboard/expiration-overview{query}", cancellationToken);
    }

    public Task<ReceiveStockResultModel> ReceiveStockAsync(ReceiveStockRequestModel request, CancellationToken cancellationToken = default) =>
        SendAsync<ReceiveStockRequestModel, ReceiveStockResultModel>(HttpMethod.Post, "/api/inventory/receive", request, cancellationToken);

    public Task<DispatchStockResultModel> DispatchStockAsync(DispatchStockRequestModel request, CancellationToken cancellationToken = default) =>
        SendAsync<DispatchStockRequestModel, DispatchStockResultModel>(HttpMethod.Post, "/api/inventory/dispatch", request, cancellationToken);

    public Task<AdjustStockResultModel> AdjustStockAsync(AdjustStockRequestModel request, CancellationToken cancellationToken = default) =>
        SendAsync<AdjustStockRequestModel, AdjustStockResultModel>(HttpMethod.Post, "/api/inventory/adjustments", request, cancellationToken);

    private async Task<TResponse> GetAsync<TResponse>(string relativeUrl, CancellationToken cancellationToken)
    {
        using var request = CreateRequest(HttpMethod.Get, relativeUrl);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("API response payload was empty.");
    }

    private async Task<TResponse> SendAsync<TRequest, TResponse>(
        HttpMethod method,
        string relativeUrl,
        TRequest payload,
        CancellationToken cancellationToken)
    {
        using var request = CreateRequest(method, relativeUrl);
        request.Content = JsonContent.Create(payload);
        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("API response payload was empty.");
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string relativeUrl)
    {
        var request = new HttpRequestMessage(method, relativeUrl);
        authSessionService.Apply(request.Headers);
        return request;
    }
}
