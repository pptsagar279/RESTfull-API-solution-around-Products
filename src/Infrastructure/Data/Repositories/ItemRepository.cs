using Microsoft.EntityFrameworkCore;
using ProductAPI.Domain.Entities;
using ProductAPI.Application.Interfaces;

namespace ProductAPI.Infrastructure.Data.Repositories;

/// <summary>
/// Item repository implementation
/// </summary>
public class ItemRepository : GenericRepository<Item>, IItemRepository
{
    public ItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Item>> GetItemsByProductIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(i => i.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Item>> GetItemsWithProductAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(i => i.Product)
            .ToListAsync(cancellationToken);
    }

}
