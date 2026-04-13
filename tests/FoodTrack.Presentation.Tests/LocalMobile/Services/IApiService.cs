namespace FoodTrack.Mobile.Services;

public interface IApiService
{
    Task<OperatorLoginResponse> LoginAsync(string badgeCode, string pin, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductSummary>> GetProductsAsync(CancellationToken cancellationToken = default);
    Task<ProductDetailModel> GetProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<BatchDetailModel> GetBatchAsync(Guid batchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BatchSummary>> GetFifoBatchesAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<DashboardOverview> GetDashboardAsync(DateTime? asOfUtc = null, CancellationToken cancellationToken = default);
    Task<ReceiveStockResultModel> ReceiveStockAsync(ReceiveStockRequestModel request, CancellationToken cancellationToken = default);
    Task<DispatchStockResultModel> DispatchStockAsync(DispatchStockRequestModel request, CancellationToken cancellationToken = default);
    Task<AdjustStockResultModel> AdjustStockAsync(AdjustStockRequestModel request, CancellationToken cancellationToken = default);
}
