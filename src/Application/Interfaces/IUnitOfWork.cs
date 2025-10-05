namespace ProductAPI.Application.Interfaces;

/// <summary>
/// Unit of Work interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IItemRepository Items { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

