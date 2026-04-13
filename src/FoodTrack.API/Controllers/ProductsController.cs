using FoodTrack.Application.Inventory;
using FoodTrack.Application.Management;
using FoodTrack.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodTrack.API.Controllers;

/// <summary>
/// Exposes product catalog endpoints for warehouse operators.
/// </summary>
[ApiController]
[Route("api/products")]
public sealed class ProductsController(
    IInventoryQueryService inventoryQueryService,
    IWarehouseManagementService warehouseManagementService) : ControllerBase
{
    /// <summary>
    /// Returns all tracked products.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ProductListItemDto>>> GetProducts(CancellationToken cancellationToken)
    {
        var products = await inventoryQueryService.GetProductsAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Returns one tracked product by identifier.
    /// </summary>
    [HttpGet("{productId:guid}")]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> GetProduct(Guid productId, CancellationToken cancellationToken)
    {
        var product = await warehouseManagementService.GetProductByIdAsync(productId, cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>
    /// Creates one product in the catalog.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProductDetailDto>> CreateProduct(
        [FromBody] UpsertProductRequest request,
        CancellationToken cancellationToken)
    {
        var category = ParseEnum<ProductCategory>(request.Category, nameof(request.Category));
        var unit = ParseEnum<UnitOfMeasure>(request.Unit, nameof(request.Unit));
        var product = await warehouseManagementService.CreateProductAsync(
            new CreateProductCommand(request.Name, request.Sku, category, unit, request.MinStockLevel),
            cancellationToken);
        return Ok(product);
    }

    /// <summary>
    /// Updates one product in the catalog.
    /// </summary>
    [HttpPut("{productId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(ProductDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailDto>> UpdateProduct(
        Guid productId,
        [FromBody] UpsertProductRequest request,
        CancellationToken cancellationToken)
    {
        var category = ParseEnum<ProductCategory>(request.Category, nameof(request.Category));
        var unit = ParseEnum<UnitOfMeasure>(request.Unit, nameof(request.Unit));
        var product = await warehouseManagementService.UpdateProductAsync(
            productId,
            new UpdateProductCommand(request.Name, request.Sku, category, unit, request.MinStockLevel),
            cancellationToken);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>
    /// Deletes one product from the catalog.
    /// </summary>
    [HttpDelete("{productId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken)
    {
        var deleted = await warehouseManagementService.DeleteProductAsync(productId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private static TEnum ParseEnum<TEnum>(string rawValue, string paramName)
        where TEnum : struct, Enum
    {
        return Enum.TryParse<TEnum>(rawValue, ignoreCase: true, out var parsed)
            ? parsed
            : throw new ArgumentException("Unsupported enum value.", paramName);
    }
}

/// <summary>
/// Captures the editable product payload sent over HTTP.
/// </summary>
public sealed record UpsertProductRequest(
    string Name,
    string Sku,
    string Category,
    string Unit,
    int MinStockLevel);
