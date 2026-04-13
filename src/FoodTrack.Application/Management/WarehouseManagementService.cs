using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;

namespace FoodTrack.Application.Management;

/// <summary>
/// Implements management-oriented product, batch, and stock-movement workflows.
/// </summary>
public sealed class WarehouseManagementService(
    IProductRepository productRepository,
    IBatchRepository batchRepository,
    IStockMovementRepository stockMovementRepository,
    IInventoryUnitOfWork unitOfWork) : IWarehouseManagementService
{
    /// <inheritdoc />
    public async Task<ProductDetailDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        var batches = await batchRepository.ListByProductAsync(productId, cancellationToken);
        return MapProduct(product, batches.Sum(batch => batch.Quantity));
    }

    /// <inheritdoc />
    public async Task<ProductDetailDto> CreateProductAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = Product.Create(command.Name, command.Sku, command.Category, command.Unit, command.MinStockLevel);
        await productRepository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MapProduct(product, 0m);
    }

    /// <inheritdoc />
    public async Task<ProductDetailDto?> UpdateProductAsync(Guid productId, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return null;
        }

        product.Update(command.Name, command.Sku, command.Category, command.Unit, command.MinStockLevel);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var batches = await batchRepository.ListByProductAsync(productId, cancellationToken);
        return MapProduct(product, batches.Sum(batch => batch.Quantity));
    }

    /// <inheritdoc />
    public async Task<bool> DeleteProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(productId, cancellationToken);
        if (product is null)
        {
            return false;
        }

        var batches = await batchRepository.ListByProductAsync(productId, cancellationToken);
        if (batches.Count > 0)
        {
            throw new InvalidOperationException("Product cannot be deleted while batches still exist.");
        }

        await productRepository.DeleteAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<BatchDetailDto?> GetBatchByIdAsync(Guid batchId, DateTime asOfUtc, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch is null)
        {
            return null;
        }

        var product = await productRepository.GetByIdAsync(batch.ProductId, cancellationToken)
            ?? throw new InvalidOperationException("Product was not found for batch detail.");
        var movements = await stockMovementRepository.ListByBatchAsync(batchId, cancellationToken);
        return MapBatch(batch, product, movements, NormalizeUtc(asOfUtc));
    }

    /// <inheritdoc />
    public async Task<BatchDetailDto?> UpdateBatchAsync(Guid batchId, UpdateBatchCommand command, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch is null)
        {
            return null;
        }

        batch.ApplyWarehouseUpdate(command.Location, command.Quantity, command.Status, command.UpdatedAtUtc);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await GetBatchByIdAsync(batchId, command.UpdatedAtUtc, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteBatchAsync(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch is null)
        {
            return false;
        }

        await stockMovementRepository.DeleteByBatchIdAsync(batchId, cancellationToken);
        await batchRepository.DeleteAsync(batch, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public async Task<BatchDetailDto?> RecallBatchAsync(
        Guid batchId,
        string performedBy,
        DateTime recalledAtUtc,
        CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetByIdAsync(batchId, cancellationToken);
        if (batch is null)
        {
            return null;
        }

        if (batch.Status != BatchStatus.Recalled)
        {
            if (batch.Quantity <= 0m)
            {
                throw new InvalidOperationException("Only batches with remaining quantity can be recalled.");
            }

            batch.Recall();
            await stockMovementRepository.AddAsync(
                StockMovement.Create(
                    batch.Id,
                    StockMovementType.Recall,
                    batch.Quantity,
                    recalledAtUtc,
                    "Sarza stiahnuta z obehu.",
                    performedBy),
                cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await GetBatchByIdAsync(batchId, recalledAtUtc, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StockMovementDto>> GetStockMovementsAsync(Guid? batchId, CancellationToken cancellationToken)
    {
        var movements = batchId.HasValue
            ? await stockMovementRepository.ListByBatchAsync(batchId.Value, cancellationToken)
            : await stockMovementRepository.ListAsync(cancellationToken);

        return movements
            .OrderByDescending(movement => movement.Timestamp)
            .Select(MapMovement)
            .ToArray();
    }

    /// <inheritdoc />
    public async Task<StockMovementDto?> GetStockMovementByIdAsync(Guid movementId, CancellationToken cancellationToken)
    {
        var movement = await stockMovementRepository.GetByIdAsync(movementId, cancellationToken);
        return movement is null ? null : MapMovement(movement);
    }

    private static ProductDetailDto MapProduct(Product product, decimal currentStockQuantity)
    {
        return new ProductDetailDto
        {
            Id = product.Id,
            Name = product.Name,
            Sku = product.Sku,
            Category = product.Category.ToString(),
            Unit = product.Unit.ToString(),
            MinStockLevel = product.MinStockLevel,
            CurrentStockQuantity = currentStockQuantity
        };
    }

    private static BatchDetailDto MapBatch(Batch batch, Product product, IReadOnlyList<StockMovement> movements, DateTime asOfUtc)
    {
        var referenceDate = asOfUtc.Date;
        return new BatchDetailDto
        {
            Id = batch.Id,
            ProductId = batch.ProductId,
            ProductName = product.Name,
            ProductSku = product.Sku,
            BatchNumber = batch.BatchNumber,
            ManufactureDate = batch.ManufactureDate,
            ExpirationDate = batch.ExpirationDate,
            Quantity = batch.Quantity,
            Location = batch.Location,
            Status = batch.Status.ToString(),
            ExpirationAlert = batch.GetExpirationAlert(asOfUtc).ToString(),
            DaysUntilExpiration = (int)(batch.ExpirationDate.Date - referenceDate).TotalDays,
            Movements = movements
                .OrderByDescending(movement => movement.Timestamp)
                .Select(MapMovement)
                .ToArray()
        };
    }

    private static StockMovementDto MapMovement(StockMovement movement)
    {
        return new StockMovementDto
        {
            Id = movement.Id,
            BatchId = movement.BatchId,
            Type = movement.Type.ToString(),
            Quantity = movement.Quantity,
            Timestamp = movement.Timestamp,
            Note = movement.Note,
            PerformedBy = movement.PerformedBy
        };
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
}
