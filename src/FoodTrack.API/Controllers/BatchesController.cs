using FoodTrack.Application.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace FoodTrack.API.Controllers;

/// <summary>
/// Exposes batch-focused warehouse read endpoints.
/// </summary>
[ApiController]
[Route("api/products/{productId:guid}/fifo-batches")]
public sealed class BatchesController(IInventoryQueryService inventoryQueryService) : ControllerBase
{
    /// <summary>
    /// Returns dispatch-ready batches in FIFO order for the supplied product.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<BatchListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<BatchListItemDto>>> GetFifoBatches(
        Guid productId,
        [FromQuery] DateTime? asOfUtc,
        CancellationToken cancellationToken)
    {
        var effectiveDate = asOfUtc ?? DateTime.UtcNow;
        var batches = await inventoryQueryService.GetFifoBatchesAsync(productId, effectiveDate, cancellationToken);
        return Ok(batches);
    }
}
