using FoodTrack.Application.Inventory.Commands;
using FoodTrack.Application.Inventory.Results;

namespace FoodTrack.Application.Inventory;

/// <summary>
/// Exposes write-side inventory workflows.
/// </summary>
public interface IInventoryCommandService
{
    /// <summary>
    /// Receives a new stock batch into the warehouse.
    /// </summary>
    Task<ReceiveStockResult> ReceiveAsync(ReceiveStockCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Dispatches product stock using FIFO rotation.
    /// </summary>
    Task<DispatchStockResult> DispatchAsync(DispatchStockCommand command, CancellationToken cancellationToken);

    /// <summary>
    /// Applies a counted stock adjustment to one batch.
    /// </summary>
    Task<AdjustStockResult> AdjustAsync(AdjustStockCommand command, CancellationToken cancellationToken);
}
