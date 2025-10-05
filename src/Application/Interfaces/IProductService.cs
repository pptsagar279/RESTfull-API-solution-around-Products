using ProductAPI.Application.DTOs;

namespace ProductAPI.Application.Interfaces;

/// <summary>
/// Product service interface
/// </summary>
public interface IProductService
{
    Task<PagedResponseDto<ProductDto>> GetProductsAsync(int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetProductWithItemsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, CancellationToken cancellationToken = default);
    Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken = default);
}
