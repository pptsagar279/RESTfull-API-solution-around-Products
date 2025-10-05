using ProductAPI.Domain.Entities;

namespace ProductAPI.Application.Interfaces;

/// <summary>
/// Item repository interface
/// </summary>
public interface IItemRepository : IGenericRepository<Item>
{
    Task<IEnumerable<Item>> GetItemsByProductIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Item>> GetItemsWithProductAsync(CancellationToken cancellationToken = default);
}

