using AutoMapper;
using Moq;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;
using ProductAPI.Application.Mapping;
using ProductAPI.Application.Services;
using ProductAPI.Domain.Entities;
using ProductAPI.Domain.Exceptions;
using Xunit;

namespace ProductAPI.Application.Tests.Services;

/// <summary>
/// Unit tests for ProductService
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IAppLogger<ProductService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<IAppLogger<ProductService>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
        
        _productService = new ProductService(_mockUnitOfWork.Object, _mapper, _mockLogger.Object);
    }

    #region GetProductsAsync Tests

    [Fact]
    public async Task GetProductsAsync_WithDefaultPagination_ReturnsPagedProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Product 1", CreatedBy = "User1", CreatedOn = DateTime.UtcNow },
            new Product { ProductId = 2, ProductName = "Product 2", CreatedBy = "User2", CreatedOn = DateTime.UtcNow }
        };

        _mockProductRepository.Setup(x => x.CountAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        
        _mockProductRepository.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Count());
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalRecords);
        Assert.Equal(1, result.TotalPages);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public async Task GetProductsAsync_WithCustomPagination_ReturnsCorrectPage()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { ProductId = 11, ProductName = "Product 11", CreatedBy = "User", CreatedOn = DateTime.UtcNow }
        };

        _mockProductRepository.Setup(x => x.CountAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(25);
        
        _mockProductRepository.Setup(x => x.GetPagedAsync(3, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetProductsAsync(page: 3, pageSize: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(25, result.TotalRecords);
        Assert.Equal(5, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public async Task GetProductsAsync_EmptyDatabase_ReturnsEmptyPagedResult()
    {
        // Arrange
        _mockProductRepository.Setup(x => x.CountAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        
        _mockProductRepository.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await _productService.GetProductsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Data);
        Assert.Equal(0, result.TotalRecords);
        Assert.Equal(0, result.TotalPages);
    }

    #endregion

    #region GetProductByIdAsync Tests

    [Fact]
    public async Task GetProductByIdAsync_ExistingProduct_ReturnsProductDto()
    {
        // Arrange
        var productId = 1;
        var product = new Product
        {
            ProductId = productId,
            ProductName = "Test Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal("Test Product", result.ProductName);
        Assert.Equal("TestUser", result.CreatedBy);
    }

    [Fact]
    public async Task GetProductByIdAsync_NonExistingProduct_ReturnsNull()
    {
        // Arrange
        var productId = 999;
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetProductByIdAsync(productId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetProductWithItemsAsync Tests

    [Fact]
    public async Task GetProductWithItemsAsync_ExistingProduct_ReturnsProductWithItems()
    {
        // Arrange
        var productId = 1;
        var product = new Product
        {
            ProductId = productId,
            ProductName = "Test Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow,
            Items = new List<Item>
            {
                new Item { ItemId = 1, ProductId = productId, Quantity = 10 },
                new Item { ItemId = 2, ProductId = productId, Quantity = 20 }
            }
        };

        _mockProductRepository.Setup(x => x.GetProductWithItemsAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductWithItemsAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal("Test Product", result.ProductName);
    }

    [Fact]
    public async Task GetProductWithItemsAsync_NonExistingProduct_ReturnsNull()
    {
        // Arrange
        var productId = 999;
        _mockProductRepository.Setup(x => x.GetProductWithItemsAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetProductWithItemsAsync(productId);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region SearchProductsAsync Tests

    [Fact]
    public async Task SearchProductsAsync_WithMatchingProducts_ReturnsProducts()
    {
        // Arrange
        var searchTerm = "Laptop";
        var products = new List<Product>
        {
            new Product { ProductId = 1, ProductName = "Laptop Pro", CreatedBy = "User1", CreatedOn = DateTime.UtcNow },
            new Product { ProductId = 2, ProductName = "Gaming Laptop", CreatedBy = "User2", CreatedOn = DateTime.UtcNow }
        };

        _mockProductRepository.Setup(x => x.SearchProductsByNameAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productService.SearchProductsAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchProductsAsync_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var searchTerm = "NonExistent";
        _mockProductRepository.Setup(x => x.SearchProductsByNameAsync(searchTerm, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Product>());

        // Act
        var result = await _productService.SearchProductsAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region CreateProductAsync Tests

    [Fact]
    public async Task CreateProductAsync_ValidProduct_ReturnsCreatedProduct()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            ProductName = "New Product",
            CreatedBy = "TestUser"
        };

        _mockProductRepository.Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _productService.CreateProductAsync(createProductDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.ProductName);
        Assert.Equal("TestUser", result.CreatedBy);
        _mockProductRepository.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateProductAsync Tests

    [Fact]
    public async Task UpdateProductAsync_ExistingProduct_ReturnsUpdatedProduct()
    {
        // Arrange
        var productId = 1;
        var existingProduct = new Product
        {
            ProductId = productId,
            ProductName = "Old Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        var updateProductDto = new UpdateProductDto
        {
            ProductName = "Updated Product",
            ModifiedBy = "UpdatedUser"
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProduct);
        
        _mockProductRepository.Setup(x => x.Update(It.IsAny<Product>()));
        
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _productService.UpdateProductAsync(productId, updateProductDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Product", result.ProductName);
        Assert.Equal("UpdatedUser", result.ModifiedBy);
        _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProductAsync_NonExistingProduct_ThrowsProductNotFoundException()
    {
        // Arrange
        var productId = 999;
        var updateProductDto = new UpdateProductDto
        {
            ProductName = "Updated Product",
            ModifiedBy = "UpdatedUser"
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() => 
            _productService.UpdateProductAsync(productId, updateProductDto));
        
        _mockProductRepository.Verify(x => x.Update(It.IsAny<Product>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeleteProductAsync Tests

    [Fact]
    public async Task DeleteProductAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        var productId = 1;
        var product = new Product
        {
            ProductId = productId,
            ProductName = "Test Product",
            CreatedBy = "TestUser",
            CreatedOn = DateTime.UtcNow
        };

        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        
        _mockProductRepository.Setup(x => x.Remove(It.IsAny<Product>()));
        
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _productService.DeleteProductAsync(productId);

        // Assert
        Assert.True(result);
        _mockProductRepository.Verify(x => x.Remove(It.IsAny<Product>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProductAsync_NonExistingProduct_ReturnsFalse()
    {
        // Arrange
        var productId = 999;
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.DeleteProductAsync(productId);

        // Assert
        Assert.False(result);
        _mockProductRepository.Verify(x => x.Remove(It.IsAny<Product>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion
}
