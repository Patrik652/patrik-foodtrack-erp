using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;
using FoodTrack.Domain.Policies;

namespace FoodTrack.Application.Inventory;

/// <summary>
/// Builds inventory-focused read models from domain objects.
/// </summary>
public sealed class InventoryQueryService(IProductRepository productRepository, IBatchRepository batchRepository) : IInventoryQueryService
{
    /// <inheritdoc />
    public async Task<ExpirationDashboardDto> GetExpirationDashboardAsync(DateTime asOfUtc, CancellationToken cancellationToken)
    {
        var referenceTimestamp = NormalizeUtc(asOfUtc);
        var referenceDate = referenceTimestamp.Date;
        var products = await productRepository.ListAsync(cancellationToken);
        var batches = await batchRepository.ListAsync(cancellationToken);
        var productLookup = products.ToDictionary(product => product.Id);

        var items = batches
            .Select(batch =>
            {
                var alert = batch.GetExpirationAlert(referenceDate);
                if (alert == ExpirationAlertLevel.None)
                {
                    return null;
                }

                productLookup.TryGetValue(batch.ProductId, out var product);
                return new
                {
                    Batch = batch,
                    Alert = alert,
                    ProductName = product?.Name ?? "Unknown Product",
                    ProductSku = product?.Sku ?? "UNKNOWN"
                };
            })
            .Where(entry => entry is not null)
            .Select(entry => entry!)
            .OrderBy(entry => GetAlertRank(entry.Alert))
            .ThenBy(entry => entry.Batch.ExpirationDate)
            .ThenBy(entry => entry.ProductName, StringComparer.OrdinalIgnoreCase)
            .Select(entry => new ExpirationAlertItemDto
            {
                ProductId = entry.Batch.ProductId,
                BatchId = entry.Batch.Id,
                ProductName = entry.ProductName,
                ProductSku = entry.ProductSku,
                BatchNumber = entry.Batch.BatchNumber,
                Location = entry.Batch.Location,
                Quantity = entry.Batch.Quantity,
                ExpirationDate = entry.Batch.ExpirationDate,
                DaysUntilExpiration = (int)(entry.Batch.ExpirationDate.Date - referenceDate).TotalDays,
                Alert = entry.Alert.ToString()
            })
            .ToArray();

        return new ExpirationDashboardDto
        {
            AsOfUtc = referenceTimestamp,
            ExpiredCount = items.Count(item => item.Alert == nameof(ExpirationAlertLevel.Expired)),
            Critical7DaysCount = items.Count(item => item.Alert == nameof(ExpirationAlertLevel.Critical7Days)),
            Warning14DaysCount = items.Count(item => item.Alert == nameof(ExpirationAlertLevel.Warning14Days)),
            Notice30DaysCount = items.Count(item => item.Alert == nameof(ExpirationAlertLevel.Notice30Days)),
            Items = items
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LowStockAlertDto>> GetLowStockAlertsAsync(DateTime asOfUtc, CancellationToken cancellationToken)
    {
        var referenceTimestamp = NormalizeUtc(asOfUtc);
        var referenceDate = referenceTimestamp.Date;
        var products = await productRepository.ListAsync(cancellationToken);
        var batches = await batchRepository.ListAsync(cancellationToken);

        var activeQuantityByProduct = batches
            .Where(batch => IsEffectiveActiveStock(batch, referenceDate))
            .GroupBy(batch => batch.ProductId)
            .ToDictionary(group => group.Key, group => group.Sum(batch => batch.Quantity));

        return products
            .Select(product =>
            {
                activeQuantityByProduct.TryGetValue(product.Id, out var currentQuantity);
                return new LowStockAlertDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductSku = product.Sku,
                    Category = product.Category.ToString(),
                    Unit = product.Unit.ToString(),
                    CurrentQuantity = currentQuantity,
                    MinStockLevel = product.MinStockLevel,
                    ShortageQuantity = Math.Max(product.MinStockLevel - currentQuantity, 0m)
                };
            })
            .Where(alert => alert.CurrentQuantity < alert.MinStockLevel)
            .OrderByDescending(alert => alert.ShortageQuantity)
            .ThenBy(alert => alert.ProductName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ProductListItemDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await productRepository.ListAsync(cancellationToken);

        return products
            .OrderBy(product => product.Name, StringComparer.OrdinalIgnoreCase)
            .Select(product => new ProductListItemDto
            {
                Id = product.Id,
                Name = product.Name,
                Sku = product.Sku,
                Category = product.Category.ToString(),
                Unit = product.Unit.ToString(),
                MinStockLevel = product.MinStockLevel
            })
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BatchListItemDto>> GetFifoBatchesAsync(
        Guid productId,
        DateTime asOfUtc,
        CancellationToken cancellationToken)
    {
        var batches = await batchRepository.ListByProductAsync(productId, cancellationToken);

        return BatchRotationPolicy.SortForDispatch(batches, asOfUtc)
            .Select(batch => new BatchListItemDto
            {
                Id = batch.Id,
                BatchNumber = batch.BatchNumber,
                Quantity = batch.Quantity,
                Location = batch.Location,
                ExpirationDate = batch.ExpirationDate,
                Status = batch.Status.ToString(),
                ExpirationAlert = batch.GetExpirationAlert(asOfUtc).ToString()
            })
            .ToArray();
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => value.ToUniversalTime()
        };
    }

    private static int GetAlertRank(ExpirationAlertLevel alert)
    {
        return alert switch
        {
            ExpirationAlertLevel.Expired => 0,
            ExpirationAlertLevel.Critical7Days => 1,
            ExpirationAlertLevel.Warning14Days => 2,
            ExpirationAlertLevel.Notice30Days => 3,
            _ => 4
        };
    }

    private static bool IsEffectiveActiveStock(Batch batch, DateTime referenceDate)
    {
        return batch.Quantity > 0m
            && batch.Status != BatchStatus.Recalled
            && batch.ExpirationDate.Date >= referenceDate;
    }
}
