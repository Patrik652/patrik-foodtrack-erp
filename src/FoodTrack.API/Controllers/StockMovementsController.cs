using FoodTrack.Application.Management;
using Microsoft.AspNetCore.Mvc;

namespace FoodTrack.API.Controllers;

/// <summary>
/// Exposes stock movement history endpoints.
/// </summary>
[ApiController]
[Route("api/stock-movements")]
public sealed class StockMovementsController(IWarehouseManagementService warehouseManagementService) : ControllerBase
{
    /// <summary>
    /// Returns stock movements, optionally filtered by batch identifier.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<StockMovementDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<StockMovementDto>>> GetStockMovements(
        [FromQuery] Guid? batchId,
        CancellationToken cancellationToken)
    {
        var movements = await warehouseManagementService.GetStockMovementsAsync(batchId, cancellationToken);
        return Ok(movements);
    }

    /// <summary>
    /// Returns one stock movement by identifier.
    /// </summary>
    [HttpGet("{movementId:guid}")]
    [ProducesResponseType(typeof(StockMovementDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockMovementDto>> GetStockMovement(Guid movementId, CancellationToken cancellationToken)
    {
        var movement = await warehouseManagementService.GetStockMovementByIdAsync(movementId, cancellationToken);
        return movement is null ? NotFound() : Ok(movement);
    }
}
