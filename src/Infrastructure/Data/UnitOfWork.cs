using ProductAPI.Application.Interfaces;
using ProductAPI.Infrastructure.Data.Repositories;

namespace ProductAPI.Infrastructure.Data;

/// <summary>
/// Unit of Work pattern implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IProductRepository? _productRepository;
    private IItemRepository? _itemRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
    public IItemRepository Items => _itemRepository ??= new ItemRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
