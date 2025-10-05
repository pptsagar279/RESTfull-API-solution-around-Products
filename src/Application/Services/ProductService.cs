using AutoMapper;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Domain.Entities;
using ProductAPI.Domain.Exceptions;

namespace ProductAPI.Application.Services;

/// <summary>
/// Product service implementation
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAppLogger<ProductService> _logger;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IAppLogger<ProductService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResponseDto<ProductDto>> GetProductsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving products - Page: {Page}, PageSize: {PageSize}", page, pageSize);
        
        var totalRecords = await _unitOfWork.Products.CountAsync(cancellationToken: cancellationToken);
        var products = await _unitOfWork.Products.GetPagedAsync(page, pageSize, cancellationToken);
        var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        _logger.LogDebug("Retrieved {Count} products out of {Total}", productDtos.Count(), totalRecords);

        return new PagedResponseDto<ProductDto>
        {
            Data = productDtos,
            Page = page,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            HasNextPage = page < totalPages,
            HasPreviousPage = page > 1
        };
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> GetProductWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetProductWithItemsAsync(id, cancellationToken);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.SearchProductsByNameAsync(searchTerm, cancellationToken);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new product: {ProductName}", createProductDto.ProductName);
        
        var product = _mapper.Map<Product>(createProductDto);
        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Product created successfully with ID: {ProductId}", product.ProductId);
        
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating product with ID: {ProductId}", id);
        
        var existingProduct = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (existingProduct == null)
        {
            _logger.LogWarning("Product not found with ID: {ProductId}", id);
            throw new ProductNotFoundException(id);
        }

        _mapper.Map(updateProductDto, existingProduct);
        _unitOfWork.Products.Update(existingProduct);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Product updated successfully with ID: {ProductId}", id);

        return _mapper.Map<ProductDto>(existingProduct);
    }

    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting product with ID: {ProductId}", id);
        
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
        {
            _logger.LogWarning("Product not found with ID: {ProductId} for deletion", id);
            return false;
        }

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Product deleted successfully with ID: {ProductId}", id);
        return true;
    }
}
