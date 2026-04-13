using FoodTrack.Application.Management;
using FoodTrack.Domain.Enums;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodTrack.API.Controllers;

/// <summary>
/// Exposes detail and maintenance endpoints for warehouse batches.
/// </summary>
[ApiController]
[Route("api/batches")]
public sealed class WarehouseBatchesController(IWarehouseManagementService warehouseManagementService) : ControllerBase
{
    /// <summary>
    /// Returns one batch detail including movement history.
    /// </summary>
    [HttpGet("{batchId:guid}")]
    [ProducesResponseType(typeof(BatchDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchDetailDto>> GetBatch(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await warehouseManagementService.GetBatchByIdAsync(batchId, DateTime.UtcNow, cancellationToken);
        return batch is null ? NotFound() : Ok(batch);
    }

    /// <summary>
    /// Updates one batch state for warehouse maintenance scenarios.
    /// </summary>
    [HttpPut("{batchId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(BatchDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchDetailDto>> UpdateBatch(
        Guid batchId,
        [FromBody] UpdateBatchRequest request,
        CancellationToken cancellationToken)
    {
        var status = Enum.TryParse<BatchStatus>(request.Status, ignoreCase: true, out var parsed)
            ? parsed
            : throw new ArgumentException("Unsupported batch status.", nameof(request.Status));

        var batch = await warehouseManagementService.UpdateBatchAsync(
            batchId,
            new UpdateBatchCommand(request.Location, request.Quantity, status, request.UpdatedAtUtc),
            cancellationToken);

        return batch is null ? NotFound() : Ok(batch);
    }

    /// <summary>
    /// Deletes one batch and its movement history.
    /// </summary>
    [HttpDelete("{batchId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBatch(Guid batchId, CancellationToken cancellationToken)
    {
        var deleted = await warehouseManagementService.DeleteBatchAsync(batchId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    /// <summary>
    /// Marks one batch as recalled and appends an audit movement entry.
    /// </summary>
    [HttpPost("{batchId:guid}/recall")]
    [Authorize]
    [ProducesResponseType(typeof(BatchDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BatchDetailDto>> RecallBatch(Guid batchId, CancellationToken cancellationToken)
    {
        var batch = await warehouseManagementService.RecallBatchAsync(
            batchId,
            ResolvePerformedBy(),
            DateTime.UtcNow,
            cancellationToken);

        return batch is null ? NotFound() : Ok(batch);
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
/// Captures the editable batch payload sent over HTTP.
/// </summary>
public sealed record UpdateBatchRequest(
    string Location,
    decimal Quantity,
    string Status,
    DateTime UpdatedAtUtc);
