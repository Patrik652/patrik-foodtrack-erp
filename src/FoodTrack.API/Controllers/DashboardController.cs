using FoodTrack.Application.Inventory;
using Microsoft.AspNetCore.Mvc;

namespace FoodTrack.API.Controllers;

/// <summary>
/// Exposes warehouse dashboard endpoints.
/// </summary>
[ApiController]
[Route("api/dashboard")]
public sealed class DashboardController(IInventoryQueryService inventoryQueryService) : ControllerBase
{
    /// <summary>
    /// Returns an expiration-focused dashboard snapshot for warehouse operators.
    /// </summary>
    [HttpGet("expiration-overview")]
    [ProducesResponseType(typeof(ExpirationDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ExpirationDashboardDto>> GetExpirationOverview(
        [FromQuery] DateTime? asOfUtc,
        CancellationToken cancellationToken)
    {
        var dashboard = await inventoryQueryService.GetExpirationDashboardAsync(
            asOfUtc ?? DateTime.UtcNow,
            cancellationToken);

        return Ok(dashboard);
    }
}
