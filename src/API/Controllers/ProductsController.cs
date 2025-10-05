using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Infrastructure.Authorization;

namespace ProductAPI.API.Controllers;

/// <summary>
/// Products controller for managing product operations
/// </summary>
[ApiController]
[Route("api/v1/[controller]/[action]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    /// <summary>
    /// Get all products with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of products</returns>
    [HttpGet]
    [ReadAccess]
    public async Task<ActionResult<PagedResponseDto<ProductDto>>> GetAll(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting products with page: {Page}, pageSize: {PageSize}", page, pageSize);
        
        var result = await _productService.GetProductsAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }


    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product details</returns>
    [HttpGet("{id:int}")]
    [ReadAccess]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);
        
        var product = await _productService.GetProductByIdAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found");
        }

        return Ok(product);
    }

    /// <summary>
    /// Get product with its items
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Product with items</returns>
    [HttpGet("{id:int}")]
    [ReadAccess]
    public async Task<ActionResult<ProductDto>> GetWithItems(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting product with items for ID: {ProductId}", id);
        
        var product = await _productService.GetProductWithItemsAsync(id, cancellationToken);
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found");
        }

        return Ok(product);
    }

    /// <summary>
    /// Search products by name
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching products</returns>
    [HttpGet]
    [ReadAccess]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchByName(
        [FromQuery] string searchTerm, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return BadRequest("Search term cannot be empty");
        }

        _logger.LogInformation("Searching products with term: {SearchTerm}", searchTerm);
        
        var products = await _productService.SearchProductsAsync(searchTerm, cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="createProductDto">Product creation data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created product</returns>
    [HttpPost]
    [WriteAccess]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductDto createProductDto, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new product: {ProductName}", createProductDto.ProductName);
        
        var product = await _productService.CreateProductAsync(createProductDto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.ProductId }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="updateProductDto">Product update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id:int}")]
    [WriteAccess]
    public async Task<ActionResult<ProductDto>> Update(
        int id, 
        [FromBody] UpdateProductDto updateProductDto, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", id);
        
        var product = await _productService.UpdateProductAsync(id, updateProductDto, cancellationToken);
        return Ok(product);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{id:int}")]
    [DeleteAccess]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        
        var result = await _productService.DeleteProductAsync(id, cancellationToken);
        if (!result)
        {
            return NotFound($"Product with ID {id} not found");
        }

        return NoContent();
    }
}
