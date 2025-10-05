using Microsoft.EntityFrameworkCore;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Data;
using ProductAPI.Infrastructure.Data.Repositories;
using Xunit;

namespace ProductAPI.Infrastructure.Tests.Repositories;

public class ProductRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
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

        // Act
        var result = await _repository.GetByIdAsync(product.ProductId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.ProductId, result.ProductId);
        Assert.Equal("Test Product", result.ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingProduct_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ValidProduct_AddsProduct()
    {
        // Arrange
        var product = new Product
        {
            ProductName = "New Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        // Act
        await _repository.AddAsync(product);
        await _context.SaveChangesAsync();

        // Assert
        var savedProduct = await _context.Products.FindAsync(product.ProductId);
        Assert.NotNull(savedProduct);
        Assert.Equal("New Product", savedProduct.ProductName);
    }

    [Fact]
    public async Task GetProductWithItemsAsync_ProductWithItems_ReturnsProductWithItems()
    {
        // Arrange
        var product = new Product
        {
            ProductName = "Product with Items",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        var item1 = new Item { ProductId = product.ProductId, Quantity = 10 };
        var item2 = new Item { ProductId = product.ProductId, Quantity = 20 };

        product.Items.Add(item1);
        product.Items.Add(item2);

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductWithItemsAsync(product.ProductId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.ProductId, result.ProductId);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.Quantity == 10);
        Assert.Contains(result.Items, i => i.Quantity == 20);
    }

    [Fact]
    public async Task SearchProductsByNameAsync_MatchingProducts_ReturnsMatchingProducts()
    {
        // Arrange
        var product1 = new Product
        {
            ProductName = "Apple iPhone",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        var product2 = new Product
        {
            ProductName = "Samsung Galaxy",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        var product3 = new Product
        {
            ProductName = "Apple iPad",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        _context.Products.AddRange(product1, product2, product3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchProductsByNameAsync("Apple");

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Contains("Apple", p.ProductName));
    }

    [Fact]
    public async Task GetProductsCreatedBetweenAsync_ProductsInDateRange_ReturnsProductsInRange()
    {
        // Arrange
        var startDate = DateTime.UtcNow.AddDays(-10);
        var endDate = DateTime.UtcNow.AddDays(-1);

        var product1 = new Product
        {
            ProductName = "Old Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow.AddDays(-15) // Outside range
        };

        var product2 = new Product
        {
            ProductName = "Recent Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow.AddDays(-5) // Inside range
        };

        var product3 = new Product
        {
            ProductName = "New Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow // Outside range
        };

        _context.Products.AddRange(product1, product2, product3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductsCreatedBetweenAsync(startDate, endDate);

        // Assert
        Assert.Single(result);
        Assert.Equal("Recent Product", result.First().ProductName);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
