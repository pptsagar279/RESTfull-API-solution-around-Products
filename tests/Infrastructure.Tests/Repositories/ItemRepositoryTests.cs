using Microsoft.EntityFrameworkCore;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Data;
using ProductAPI.Infrastructure.Data.Repositories;
using Xunit;

namespace ProductAPI.Infrastructure.Tests.Repositories;

public class ItemRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ItemRepository _repository;

    public ItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ItemRepository(_context);
    }

    [Fact]
    public async Task GetItemsByProductIdAsync_ExistingProduct_ReturnsItems()
    {
        // Arrange
        var product = new Product
        {
            ProductName = "Test Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var item1 = new Item { ProductId = product.ProductId, Quantity = 10 };
        var item2 = new Item { ProductId = product.ProductId, Quantity = 20 };
        var item3 = new Item { ProductId = product.ProductId, Quantity = 30 };

        _context.Items.AddRange(item1, item2, item3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetItemsByProductIdAsync(product.ProductId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.All(result, item => Assert.Equal(product.ProductId, item.ProductId));
        Assert.Contains(result, i => i.Quantity == 10);
        Assert.Contains(result, i => i.Quantity == 20);
        Assert.Contains(result, i => i.Quantity == 30);
    }

    [Fact]
    public async Task GetItemsByProductIdAsync_NonExistingProduct_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetItemsByProductIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetItemsWithProductAsync_ItemsWithProducts_ReturnsItemsWithProducts()
    {
        // Arrange
        var product1 = new Product
        {
            ProductName = "Product 1",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        var product2 = new Product
        {
            ProductName = "Product 2",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        _context.Products.AddRange(product1, product2);
        await _context.SaveChangesAsync();

        var item1 = new Item { ProductId = product1.ProductId, Quantity = 10 };
        var item2 = new Item { ProductId = product2.ProductId, Quantity = 20 };

        _context.Items.AddRange(item1, item2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetItemsWithProductAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, item => Assert.NotNull(item.Product));
        Assert.Contains(result, i => i.Product.ProductName == "Product 1");
        Assert.Contains(result, i => i.Product.ProductName == "Product 2");
    }

    [Fact]
    public async Task GetItemsWithProductAsync_NoItems_ReturnsEmptyList()
    {
        // Act
        var result = await _repository.GetItemsWithProductAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }


    [Fact]
    public async Task GetItemsByProductIdAsync_MultipleProducts_ReturnsOnlyItemsForSpecificProduct()
    {
        // Arrange
        var product1 = new Product
        {
            ProductName = "Product 1",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        var product2 = new Product
        {
            ProductName = "Product 2",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        _context.Products.AddRange(product1, product2);
        await _context.SaveChangesAsync();

        var item1 = new Item { ProductId = product1.ProductId, Quantity = 10 };
        var item2 = new Item { ProductId = product1.ProductId, Quantity = 20 };
        var item3 = new Item { ProductId = product2.ProductId, Quantity = 30 };

        _context.Items.AddRange(item1, item2, item3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetItemsByProductIdAsync(product1.ProductId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, item => Assert.Equal(product1.ProductId, item.ProductId));
        Assert.Contains(result, i => i.Quantity == 10);
        Assert.Contains(result, i => i.Quantity == 20);
        Assert.DoesNotContain(result, i => i.Quantity == 30);
    }

    [Fact]
    public async Task GetItemsWithProductAsync_ItemsWithDifferentProducts_ReturnsAllItemsWithCorrectProducts()
    {
        // Arrange
        var product1 = new Product
        {
            ProductName = "Laptop",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        var product2 = new Product
        {
            ProductName = "Phone",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        _context.Products.AddRange(product1, product2);
        await _context.SaveChangesAsync();

        var item1 = new Item { ProductId = product1.ProductId, Quantity = 5 };
        var item2 = new Item { ProductId = product2.ProductId, Quantity = 10 };
        var item3 = new Item { ProductId = product1.ProductId, Quantity = 3 };

        _context.Items.AddRange(item1, item2, item3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetItemsWithProductAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        
        var laptopItems = result.Where(i => i.Product.ProductName == "Laptop").ToList();
        var phoneItems = result.Where(i => i.Product.ProductName == "Phone").ToList();
        
        Assert.Equal(2, laptopItems.Count);
        Assert.Single(phoneItems);
        Assert.Contains(laptopItems, i => i.Quantity == 5);
        Assert.Contains(laptopItems, i => i.Quantity == 3);
        Assert.Contains(phoneItems, i => i.Quantity == 10);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
