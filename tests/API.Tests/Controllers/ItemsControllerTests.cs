using ProductAPI.Application.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace ProductAPI.API.Tests.Controllers;

/// <summary>
/// Integration tests for ItemsController
/// </summary>
public class ItemsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ItemsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    #region Authorization Tests

    [Fact]
    public async Task GetItems_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/items");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateItem_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createItemDto = new CreateItemDto
        {
            ProductId = 1,
            Quantity = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/items", createItemDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Get Items Tests

    [Fact]
    public async Task GetItems_WithAuthentication_ReturnsPagedItems()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/items");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponseDto<ItemDto>>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(pagedResponse);
        Assert.NotNull(pagedResponse.Data);
    }

    [Fact]
    public async Task GetItems_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/items?page=1&pageSize=5");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var pagedResponse = await response.Content.ReadFromJsonAsync<PagedResponseDto<ItemDto>>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(pagedResponse);
        Assert.Equal(1, pagedResponse.Page);
        Assert.Equal(5, pagedResponse.PageSize);
    }

    #endregion

    #region Get Item by ID Tests

    [Fact]
    public async Task GetItem_WithValidId_ReturnsItem()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productId = await CreateTestProductAsync();
        var createItemDto = new CreateItemDto
        {
            ProductId = productId,
            Quantity = 5
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/items", createItemDto);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Act
        var response = await _client.GetAsync($"/api/v1/items/{createdItem!.ItemId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var itemDto = await response.Content.ReadFromJsonAsync<ItemDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(itemDto);
        Assert.Equal(createdItem.ItemId, itemDto.ItemId);
        Assert.Equal(5, itemDto.Quantity);
    }

    [Fact]
    public async Task GetItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/items/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion


    #region Get Items by Product ID Tests

    [Fact]
    public async Task GetItemsByProductId_WithValidProductId_ReturnsItems()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productId = await CreateTestProductAsync();

        for (int i = 1; i <= 3; i++)
        {
            var createItemDto = new CreateItemDto
            {
                ProductId = productId,
                Quantity = i * 10
            };
            await _client.PostAsJsonAsync("/api/v1/items", createItemDto);
        }

        // Act
        var response = await _client.GetAsync($"/api/v1/items/by-product/{productId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var items = await response.Content.ReadFromJsonAsync<IEnumerable<ItemDto>>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(items);
        Assert.True(items.Count() >= 3);
        Assert.All(items, item => Assert.Equal(productId, item.ProductId));
    }

    [Fact]
    public async Task GetItemsByProductId_WithNonExistentProductId_ReturnsEmptyList()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/items/by-product/999999");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var items = await response.Content.ReadFromJsonAsync<IEnumerable<ItemDto>>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(items);
        Assert.Empty(items);
    }

    #endregion

    #region Create Item Tests

    [Fact]
    public async Task CreateItem_WithValidData_ReturnsCreatedItem()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productId = await CreateTestProductAsync();
        var createItemDto = new CreateItemDto
        {
            ProductId = productId,
            Quantity = 20
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/items", createItemDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var itemDto = await response.Content.ReadFromJsonAsync<ItemDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(itemDto);
        Assert.Equal(20, itemDto.Quantity);
        Assert.Equal(productId, itemDto.ProductId);
        Assert.True(itemDto.ItemId > 0);

        Assert.NotNull(response.Headers.Location);
    }

    [Fact]
    public async Task CreateItem_WithNonExistentProductId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createItemDto = new CreateItemDto
        {
            ProductId = 999999,
            Quantity = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/items", createItemDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        
        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Product with ID 999999 was not found", errorContent);
    }

    #endregion

    #region Update Item Tests

    [Fact]
    public async Task UpdateItem_WithValidData_ReturnsUpdatedItem()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productId = await CreateTestProductAsync();
        var createItemDto = new CreateItemDto
        {
            ProductId = productId,
            Quantity = 10
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/items", createItemDto);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var updateItemDto = new UpdateItemDto
        {
            Quantity = 25
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/items/{createdItem!.ItemId}", updateItemDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var itemDto = await response.Content.ReadFromJsonAsync<ItemDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(itemDto);
        Assert.Equal(25, itemDto.Quantity);
    }

    [Fact]
    public async Task UpdateItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateItemDto = new UpdateItemDto
        {
            Quantity = 50
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/v1/items/999999", updateItemDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Item with ID 999999 was not found", errorContent);
    }

    #endregion

    #region Delete Item Tests

    [Fact]
    public async Task DeleteItem_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var productId = await CreateTestProductAsync();
        var createItemDto = new CreateItemDto
        {
            ProductId = productId,
            Quantity = 30
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/items", createItemDto);
        var createdItem = await createResponse.Content.ReadFromJsonAsync<ItemDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Act
        var response = await _client.DeleteAsync($"/api/v1/items/{createdItem!.ItemId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/v1/items/{createdItem.ItemId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteItem_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync("/api/v1/items/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Helper Methods

    private async Task<string> GetAuthTokenAsync()
    {
        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return loginResponse!.AccessToken;
    }

    private async Task<int> CreateTestProductAsync()
    {
        var createProductDto = new CreateProductDto
        {
            ProductName = $"Test Product {Guid.NewGuid()}",
            CreatedBy = "TestUser"
        };

        var response = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);
        var productDto = await response.Content.ReadFromJsonAsync<ProductDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return productDto!.ProductId;
    }

    #endregion
}

