using FoodTrack.Application.Abstractions.Persistence;
using FoodTrack.Application.Inventory.Commands;
using FoodTrack.Application.Inventory.Results;
using FoodTrack.Domain.Entities;
using FoodTrack.Domain.Enums;
using FoodTrack.Domain.Policies;

namespace FoodTrack.Application.Inventory;

/// <summary>
/// Executes warehouse write workflows over products, batches, and movements.
/// </summary>
public sealed class InventoryCommandService(
    IProductRepository productRepository,
    IBatchRepository batchRepository,
    IStockMovementRepository stockMovementRepository,
    IInventoryUnitOfWork unitOfWork) : IInventoryCommandService
{
    /// <inheritdoc />
    public async Task<ReceiveStockResult> ReceiveAsync(ReceiveStockCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new InvalidOperationException("Product was not found.");

        var batch = Batch.Create(
            product.Id,
            command.BatchNumber,
            command.ManufactureDate,
            command.ExpirationDate,
            command.Quantity,
            command.Location);

        var movement = StockMovement.Create(
            batch.Id,
            StockMovementType.Receive,
            command.Quantity,
            command.ReceivedAtUtc,
            command.Note,
            command.PerformedBy);

        await batchRepository.AddAsync(batch, cancellationToken);
        await stockMovementRepository.AddAsync(movement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ReceiveStockResult
        {
            BatchId = batch.Id,
            MovementId = movement.Id,
            BatchNumber = batch.BatchNumber,
            Quantity = batch.Quantity,
            Location = batch.Location
        };
    }

    /// <inheritdoc />
    public async Task<DispatchStockResult> DispatchAsync(DispatchStockCommand command, CancellationToken cancellationToken)
    {
        _ = await productRepository.GetByIdAsync(command.ProductId, cancellationToken)
            ?? throw new InvalidOperationException("Product was not found.");

        var batches = await batchRepository.ListByProductAsync(command.ProductId, cancellationToken);
        var orderedBatches = BatchRotationPolicy.SortForDispatch(batches, command.DispatchedAtUtc);
        var availableQuantity = orderedBatches.Sum(batch => batch.Quantity);
        if (availableQuantity < command.Quantity)
        {
            throw new InvalidOperationException("Insufficient stock for dispatch.");
        }

        var remainingQuantity = command.Quantity;
        var movements = new List<StockMovement>();
        var lines = new List<DispatchStockLineResult>();

        foreach (var batch in orderedBatches)
        {
            if (remainingQuantity <= 0m)
            {
                break;
            }

            var dispatchedQuantity = Math.Min(batch.Quantity, remainingQuantity);
            batch.Dispatch(dispatchedQuantity, command.DispatchedAtUtc);
            remainingQuantity -= dispatchedQuantity;

            movements.Add(StockMovement.Create(
                batch.Id,
                StockMovementType.Dispatch,
                dispatchedQuantity,
                command.DispatchedAtUtc,
                command.Note,
                command.PerformedBy));

            lines.Add(new DispatchStockLineResult
            {
                BatchId = batch.Id,
                BatchNumber = batch.BatchNumber,
                Quantity = dispatchedQuantity,
                Location = batch.Location
            });
        }

        await stockMovementRepository.AddRangeAsync(movements, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new DispatchStockResult
        {
            ProductId = command.ProductId,
            RequestedQuantity = command.Quantity,
            AllocatedQuantity = command.Quantity - remainingQuantity,
            Lines = lines
        };
    }

    /// <inheritdoc />
    public async Task<AdjustStockResult> AdjustAsync(AdjustStockCommand command, CancellationToken cancellationToken)
    {
        var batch = await batchRepository.GetByIdAsync(command.BatchId, cancellationToken)
            ?? throw new InvalidOperationException("Batch was not found.");

        var previousQuantity = batch.Quantity;
        batch.SetQuantity(command.NewQuantity, command.AdjustedAtUtc);

        var movement = StockMovement.Create(
            batch.Id,
            StockMovementType.Adjust,
            Math.Abs(previousQuantity - command.NewQuantity),
            command.AdjustedAtUtc,
            command.Note,
            command.PerformedBy);

        await stockMovementRepository.AddAsync(movement, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AdjustStockResult
        {
            BatchId = batch.Id,
            MovementId = movement.Id,
            PreviousQuantity = previousQuantity,
            NewQuantity = batch.Quantity,
            Status = batch.Status.ToString()
        };
    }
}
