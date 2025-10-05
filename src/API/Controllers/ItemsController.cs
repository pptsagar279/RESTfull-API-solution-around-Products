using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Infrastructure.Authorization;

namespace ProductAPI.API.Controllers;

/// <summary>
/// Items controller for managing item operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]/[action]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly ILogger<ItemsController> _logger;

    public ItemsController(IItemService itemService, ILogger<ItemsController> logger)
    {
        _itemService = itemService;
        _logger = logger;
    }

    /// <summary>
    /// Get all items with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of items</returns>
    [HttpGet]
    [ReadAccess]
    public async Task<ActionResult<PagedResponseDto<ItemDto>>> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting items with page: {Page}, pageSize: {PageSize}", page, pageSize);
        
        var result = await _itemService.GetItemsAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get item by ID
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Item details</returns>
    [HttpGet("{id:int}")]
    [ReadAccess]
    public async Task<ActionResult<ItemDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting item with ID: {ItemId}", id);
        
        var item = await _itemService.GetItemByIdAsync(id, cancellationToken);
        if (item == null)
        {
            return NotFound($"Item with ID {id} not found");
        }

        return Ok(item);
    }


    /// <summary>
    /// Get items by product ID
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of items for the product</returns>
    [HttpGet("{productId:int}")]
    [ReadAccess]
    public async Task<ActionResult<IEnumerable<ItemDto>>> GetByProductId(
        int productId, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting items for product ID: {ProductId}", productId);
        
        var items = await _itemService.GetItemsByProductIdAsync(productId, cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Create a new item
    /// </summary>
    /// <param name="createItemDto">Item creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created item</returns>
    [HttpPost]
    [WriteAccess]
    public async Task<ActionResult<ItemDto>> Create(
        [FromBody] CreateItemDto createItemDto, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new item for product ID: {ProductId}", createItemDto.ProductId);
        
        var item = await _itemService.CreateItemAsync(createItemDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = item.ItemId }, item);
    }

    /// <summary>
    /// Update an existing item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="updateItemDto">Item update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated item</returns>
    [HttpPut("{id:int}")]
    [WriteAccess]
    public async Task<ActionResult<ItemDto>> Update(
        int id, 
        [FromBody] UpdateItemDto updateItemDto, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating item with ID: {ItemId}", id);
        
        var item = await _itemService.UpdateItemAsync(id, updateItemDto, cancellationToken);
        return Ok(item);
    }

    /// <summary>
    /// Delete an item
    /// </summary>
    /// <param name="id">Item ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id:int}")]
    [DeleteAccess]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting item with ID: {ItemId}", id);
        
        var result = await _itemService.DeleteItemAsync(id, cancellationToken);
        if (!result)
        {
            return NotFound($"Item with ID {id} not found");
        }

        return NoContent();
    }
}
