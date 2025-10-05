using Microsoft.EntityFrameworkCore;
using ProductAPI.Domain.Entities;
using ProductAPI.Application.Interfaces;

namespace ProductAPI.Infrastructure.Data.Repositories;

/// <summary>
/// Product repository implementation
/// </summary>
public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetProductsWithItemsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetProductWithItemsAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
    }

    public async Task<IEnumerable<Product>> SearchProductsByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.ProductName.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsCreatedBetweenAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(p => p.CreatedOn >= startDate && p.CreatedOn <= endDate)
            .ToListAsync(cancellationToken);
    }
}
