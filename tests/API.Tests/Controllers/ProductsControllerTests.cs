using ProductAPI.Application.DTOs;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Xunit;

namespace ProductAPI.API.Tests.Controllers;

/// <summary>
/// Integration tests for ProductsController
/// </summary>
public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithAuthentication_ReturnsProducts()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var pagedResponse = JsonSerializer.Deserialize<PagedResponseDto<ProductDto>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(pagedResponse);
        Assert.NotNull(pagedResponse.Data);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreatedProduct()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createProductDto = new CreateProductDto
        {
            ProductName = "Test Product",
            CreatedBy = "TestUser"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var productDto = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(productDto);
        Assert.Equal("Test Product", productDto.ProductName);
        Assert.Equal("TestUser", productDto.CreatedBy);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createProductDto = new CreateProductDto
        {
            ProductName = "Test Product for Get",
            CreatedBy = "TestUser"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{createdProduct!.ProductId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var productDto = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(productDto);
        Assert.Equal(createdProduct.ProductId, productDto.ProductId);
        Assert.Equal("Test Product for Get", productDto.ProductName);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsUpdatedProduct()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createProductDto = new CreateProductDto
        {
            ProductName = "Original Product",
            CreatedBy = "TestUser"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var updateProductDto = new UpdateProductDto
        {
            ProductName = "Updated Product",
            ModifiedBy = "UpdatedUser"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/products/{createdProduct!.ProductId}", updateProductDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var productDto = JsonSerializer.Deserialize<ProductDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(productDto);
        Assert.Equal("Updated Product", productDto.ProductName);
        Assert.Equal("UpdatedUser", productDto.ModifiedBy);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsSuccessMessage()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createProductDto = new CreateProductDto
        {
            ProductName = "Product to Delete",
            CreatedBy = "TestUser"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createProductDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Act
        var response = await _client.DeleteAsync($"/api/v1/products/{createdProduct!.ProductId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<JsonElement>(content);
        
        Assert.True(responseObject.TryGetProperty("message", out var messageProperty));
        Assert.Equal($"Product with ID {createdProduct.ProductId} deleted successfully", messageProperty.GetString());
    }

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
}
