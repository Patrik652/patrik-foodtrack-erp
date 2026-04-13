using System.Security.Claims;
using FoodTrack.Application.Inventory;
using FoodTrack.Application.Inventory.Commands;
using FoodTrack.Application.Inventory.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodTrack.API.Controllers;

/// <summary>
/// Exposes write-side warehouse inventory operations.
/// </summary>
[ApiController]
[Route("api/inventory")]
[Authorize]
public sealed class InventoryController(IInventoryCommandService inventoryCommandService) : ControllerBase
{
    /// <summary>
    /// Receives a new stock batch.
    /// </summary>
    [HttpPost("receive")]
    [ProducesResponseType(typeof(ReceiveStockResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ReceiveStockResult>> Receive(
        [FromBody] ReceiveStockRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ReceiveStockCommand(
            request.ProductId,
            request.BatchNumber,
            request.ManufactureDate,
            request.ExpirationDate,
            request.Quantity,
            request.Location,
            request.Note,
            ResolvePerformedBy(),
            request.ReceivedAtUtc);

        var result = await inventoryCommandService.ReceiveAsync(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Dispatches stock using FIFO rotation.
    /// </summary>
    [HttpPost("dispatch")]
    [ProducesResponseType(typeof(DispatchStockResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DispatchStockResult>> Dispatch(
        [FromBody] DispatchStockRequest request,
        CancellationToken cancellationToken)
    {
        var command = new DispatchStockCommand(
            request.ProductId,
            request.Quantity,
            request.DispatchedAtUtc,
            request.Note,
            ResolvePerformedBy());

        var result = await inventoryCommandService.DispatchAsync(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Applies a physical count adjustment to one batch.
    /// </summary>
    [HttpPost("adjustments")]
    [ProducesResponseType(typeof(AdjustStockResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AdjustStockResult>> Adjust(
        [FromBody] AdjustStockRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AdjustStockCommand(
            request.BatchId,
            request.NewQuantity,
            request.AdjustedAtUtc,
            request.Note,
            ResolvePerformedBy());

        var result = await inventoryCommandService.AdjustAsync(command, cancellationToken);
        return Ok(result);
    }

    private string ResolvePerformedBy()
    {
        var operatorName = User.FindFirstValue(ClaimTypes.Name);
        return string.IsNullOrWhiteSpace(operatorName)
            ? throw new InvalidOperationException("Authenticated operator identity is missing.")
            : operatorName;
    }
}

/// <summary>
/// Captures the data required to receive a new warehouse batch over HTTP.
/// </summary>
public sealed record ReceiveStockRequest(
    Guid ProductId,
    string BatchNumber,
    DateTime ManufactureDate,
    DateTime ExpirationDate,
    decimal Quantity,
    string Location,
    string Note,
    DateTime ReceivedAtUtc);

/// <summary>
/// Captures the data required to dispatch stock over HTTP.
/// </summary>
public sealed record DispatchStockRequest(
    Guid ProductId,
    decimal Quantity,
    DateTime DispatchedAtUtc,
    string Note);

/// <summary>
/// Captures the data required to apply a stock adjustment over HTTP.
/// </summary>
public sealed record AdjustStockRequest(
    Guid BatchId,
    decimal NewQuantity,
    DateTime AdjustedAtUtc,
    string Note);
