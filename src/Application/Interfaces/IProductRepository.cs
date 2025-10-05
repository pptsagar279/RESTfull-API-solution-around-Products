using ProductAPI.Domain.Entities;

namespace ProductAPI.Application.Interfaces;

/// <summary>
/// Product repository interface
/// </summary>
public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetProductsWithItemsAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductWithItemsAsync(int productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> SearchProductsByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetProductsCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}

