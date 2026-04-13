using FoodTrack.Mobile.Services;

namespace FoodTrack.Presentation.Tests;

internal sealed class FakeApiService : IApiService
{
    public IReadOnlyList<ProductSummary> Products { get; init; } = [];
    public bool ThrowOnProducts { get; init; }
    public DashboardOverview Dashboard { get; init; } = new();
    public ReceiveStockResultModel ReceiveResult { get; init; } = new();

    public Task<AdjustStockResultModel> AdjustStockAsync(AdjustStockRequestModel request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new AdjustStockResultModel());

    public Task<DispatchStockResultModel> DispatchStockAsync(DispatchStockRequestModel request, CancellationToken cancellationToken = default) =>
        Task.FromResult(new DispatchStockResultModel());

    public Task<BatchDetailModel> GetBatchAsync(Guid batchId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new BatchDetailModel());

    public Task<DashboardOverview> GetDashboardAsync(DateTime? asOfUtc = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(Dashboard);

    public Task<IReadOnlyList<BatchSummary>> GetFifoBatchesAsync(Guid productId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<BatchSummary>>([]);

    public Task<ProductDetailModel> GetProductAsync(Guid productId, CancellationToken cancellationToken = default) =>
        Task.FromResult(new ProductDetailModel());

    public Task<IReadOnlyList<ProductSummary>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        if (ThrowOnProducts)
        {
            throw new HttpRequestException("offline");
        }

        return Task.FromResult(Products);
    }

    public Task<OperatorLoginResponse> LoginAsync(string badgeCode, string pin, CancellationToken cancellationToken = default) =>
        Task.FromResult(new OperatorLoginResponse
        {
            AccessToken = "jwt",
            BadgeCode = badgeCode,
            OperatorName = "Roman Skladnik"
        });

    public Task<ReceiveStockResultModel> ReceiveStockAsync(ReceiveStockRequestModel request, CancellationToken cancellationToken = default) =>
        Task.FromResult(ReceiveResult);
}
