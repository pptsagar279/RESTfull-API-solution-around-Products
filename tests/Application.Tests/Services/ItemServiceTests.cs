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
/// Unit tests for ItemService
/// </summary>
public class ItemServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IItemRepository> _mockItemRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<IAppLogger<ItemService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockItemRepository = new Mock<IItemRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockLogger = new Mock<IAppLogger<ItemService>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _mockUnitOfWork.Setup(x => x.Items).Returns(_mockItemRepository.Object);
        _mockUnitOfWork.Setup(x => x.Products).Returns(_mockProductRepository.Object);
        
        _itemService = new ItemService(_mockUnitOfWork.Object, _mapper, _mockLogger.Object);
    }

    #region GetItemsAsync Tests

    [Fact]
    public async Task GetItemsAsync_WithDefaultPagination_ReturnsPagedItems()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { ItemId = 1, ProductId = 1, Quantity = 10 },
            new Item { ItemId = 2, ProductId = 1, Quantity = 20 }
        };

        _mockItemRepository.Setup(x => x.CountAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);
        
        _mockItemRepository.Setup(x => x.GetPagedAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _itemService.GetItemsAsync();

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
    public async Task GetItemsAsync_WithCustomPagination_ReturnsCorrectPage()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { ItemId = 3, ProductId = 2, Quantity = 30 }
        };

        _mockItemRepository.Setup(x => x.CountAsync(null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(15);
        
        _mockItemRepository.Setup(x => x.GetPagedAsync(2, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _itemService.GetItemsAsync(page: 2, pageSize: 5);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(15, result.TotalRecords);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
    }

    #endregion

    #region GetItemByIdAsync Tests

    [Fact]
    public async Task GetItemByIdAsync_ExistingItem_ReturnsItemDto()
    {
        // Arrange
        var itemId = 1;
        var item = new Item
        {
            ItemId = itemId,
            ProductId = 1,
            Quantity = 50
        };

        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _itemService.GetItemByIdAsync(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(itemId, result.ItemId);
        Assert.Equal(1, result.ProductId);
        Assert.Equal(50, result.Quantity);
    }

    [Fact]
    public async Task GetItemByIdAsync_NonExistingItem_ReturnsNull()
    {
        // Arrange
        var itemId = 999;
        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _itemService.GetItemByIdAsync(itemId);

        // Assert
        Assert.Null(result);
    }

    #endregion


    #region GetItemsByProductIdAsync Tests

    [Fact]
    public async Task GetItemsByProductIdAsync_ExistingProduct_ReturnsItems()
    {
        // Arrange
        var productId = 1;
        var items = new List<Item>
        {
            new Item { ItemId = 1, ProductId = productId, Quantity = 10 },
            new Item { ItemId = 2, ProductId = productId, Quantity = 20 },
            new Item { ItemId = 3, ProductId = productId, Quantity = 30 }
        };

        _mockItemRepository.Setup(x => x.GetItemsByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(items);

        // Act
        var result = await _itemService.GetItemsByProductIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.All(result, item => Assert.Equal(productId, item.ProductId));
    }

    [Fact]
    public async Task GetItemsByProductIdAsync_NoItems_ReturnsEmptyList()
    {
        // Arrange
        var productId = 999;
        _mockItemRepository.Setup(x => x.GetItemsByProductIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Item>());

        // Act
        var result = await _itemService.GetItemsByProductIdAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region CreateItemAsync Tests

    [Fact]
    public async Task CreateItemAsync_ValidItem_ReturnsCreatedItem()
    {
        // Arrange
        var createItemDto = new CreateItemDto
        {
            ProductId = 1,
            Quantity = 15
        };

        _mockProductRepository.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockItemRepository.Setup(x => x.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _itemService.CreateItemAsync(createItemDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(15, result.Quantity);
        Assert.Equal(1, result.ProductId);
        
        _mockItemRepository.Verify(x => x.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateItemAsync_NonExistentProduct_ThrowsProductNotFoundException()
    {
        // Arrange
        var createItemDto = new CreateItemDto
        {
            ProductId = 999,
            Quantity = 10
        };

        _mockProductRepository.Setup(x => x.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() => 
            _itemService.CreateItemAsync(createItemDto));
        
        _mockItemRepository.Verify(x => x.AddAsync(It.IsAny<Item>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region UpdateItemAsync Tests

    [Fact]
    public async Task UpdateItemAsync_ExistingItem_ReturnsUpdatedItem()
    {
        // Arrange
        var itemId = 1;
        var existingItem = new Item
        {
            ItemId = itemId,
            ProductId = 1,
            Quantity = 10
        };

        var updateItemDto = new UpdateItemDto
        {
            Quantity = 25
        };

        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingItem);
        
        _mockItemRepository.Setup(x => x.Update(It.IsAny<Item>()));
        
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _itemService.UpdateItemAsync(itemId, updateItemDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(25, result.Quantity);
        
        _mockItemRepository.Verify(x => x.Update(It.IsAny<Item>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateItemAsync_NonExistingItem_ThrowsItemNotFoundException()
    {
        // Arrange
        var itemId = 999;
        var updateItemDto = new UpdateItemDto
        {
            Quantity = 50
        };

        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ItemNotFoundException>(() => 
            _itemService.UpdateItemAsync(itemId, updateItemDto));
        
        _mockItemRepository.Verify(x => x.Update(It.IsAny<Item>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DeleteItemAsync Tests

    [Fact]
    public async Task DeleteItemAsync_ExistingItem_ReturnsTrue()
    {
        // Arrange
        var itemId = 1;
        var item = new Item
        {
            ItemId = itemId,
            ProductId = 1,
            Quantity = 10
        };

        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);
        
        _mockItemRepository.Setup(x => x.Remove(It.IsAny<Item>()));
        
        _mockUnitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _itemService.DeleteItemAsync(itemId);

        // Assert
        Assert.True(result);
        _mockItemRepository.Verify(x => x.Remove(It.IsAny<Item>()), Times.Once);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteItemAsync_NonExistingItem_ReturnsFalse()
    {
        // Arrange
        var itemId = 999;
        _mockItemRepository.Setup(x => x.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Item?)null);

        // Act
        var result = await _itemService.DeleteItemAsync(itemId);

        // Assert
        Assert.False(result);
        _mockItemRepository.Verify(x => x.Remove(It.IsAny<Item>()), Times.Never);
        _mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion
}

