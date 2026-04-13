namespace FoodTrack.Mobile.Services;

public sealed class OperatorLoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
    public Guid OperatorId { get; init; }
    public string BadgeCode { get; init; } = string.Empty;
    public string OperatorName { get; init; } = string.Empty;
}

public class ProductSummary
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Unit { get; init; } = string.Empty;
    public int MinStockLevel { get; init; }
}

public sealed class ProductDetailModel : ProductSummary
{
    public decimal CurrentStockQuantity { get; init; }
}

public sealed class BatchSummary
{
    public Guid Id { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Location { get; init; } = string.Empty;
    public DateTime ExpirationDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string ExpirationAlert { get; init; } = string.Empty;
}

public sealed class BatchDetailModel
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSku { get; init; } = string.Empty;
    public string BatchNumber { get; init; } = string.Empty;
    public DateTime ManufactureDate { get; init; }
    public DateTime ExpirationDate { get; init; }
    public decimal Quantity { get; init; }
    public string Location { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string ExpirationAlert { get; init; } = string.Empty;
    public int DaysUntilExpiration { get; init; }
    public IReadOnlyList<StockMovementModel> Movements { get; init; } = [];
}

public sealed class StockMovementModel
{
    public Guid Id { get; init; }
    public Guid BatchId { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public DateTime Timestamp { get; init; }
    public string Note { get; init; } = string.Empty;
    public string PerformedBy { get; init; } = string.Empty;
}

public sealed class DashboardOverview
{
    public DateTime AsOfUtc { get; init; }
    public int ExpiredCount { get; init; }
    public int Critical7DaysCount { get; init; }
    public int Warning14DaysCount { get; init; }
    public int Notice30DaysCount { get; init; }
    public IReadOnlyList<ExpirationAlertModel> Items { get; init; } = [];
}

public sealed class ExpirationAlertModel
{
    public Guid ProductId { get; init; }
    public Guid BatchId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string ProductSku { get; init; } = string.Empty;
    public string BatchNumber { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public DateTime ExpirationDate { get; init; }
    public int DaysUntilExpiration { get; init; }
    public string Alert { get; init; } = string.Empty;
}

public sealed class ReceiveStockRequestModel
{
    public Guid ProductId { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public DateTime ManufactureDate { get; init; }
    public DateTime ExpirationDate { get; init; }
    public decimal Quantity { get; init; }
    public string Location { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public DateTime ReceivedAtUtc { get; init; }
}

public sealed class ReceiveStockResultModel
{
    public Guid BatchId { get; init; }
    public Guid MovementId { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Location { get; init; } = string.Empty;
}

public sealed class DispatchStockRequestModel
{
    public Guid ProductId { get; init; }
    public decimal Quantity { get; init; }
    public DateTime DispatchedAtUtc { get; init; }
    public string Note { get; init; } = string.Empty;
}

public sealed class DispatchStockResultModel
{
    public Guid ProductId { get; init; }
    public decimal RequestedQuantity { get; init; }
    public decimal AllocatedQuantity { get; init; }
    public IReadOnlyList<DispatchStockLineModel> Lines { get; init; } = [];
}

public sealed class DispatchStockLineModel
{
    public Guid BatchId { get; init; }
    public string BatchNumber { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public string Location { get; init; } = string.Empty;
}

public sealed class AdjustStockRequestModel
{
    public Guid BatchId { get; init; }
    public decimal NewQuantity { get; init; }
    public DateTime AdjustedAtUtc { get; init; }
    public string Note { get; init; } = string.Empty;
}

public sealed class AdjustStockResultModel
{
    public Guid BatchId { get; init; }
    public Guid MovementId { get; init; }
    public decimal PreviousQuantity { get; init; }
    public decimal NewQuantity { get; init; }
    public string Status { get; init; } = string.Empty;
}
