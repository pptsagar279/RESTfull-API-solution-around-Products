using ProductAPI.Application.DTOs;
using System.Net;
using System.Text.Json;
using Xunit;

namespace ProductAPI.API.Tests.Controllers;

/// <summary>
/// Integration tests for AuthController
/// </summary>
public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(loginResponse);
        Assert.NotEmpty(loginResponse.AccessToken);
        Assert.NotEmpty(loginResponse.RefreshToken);
        Assert.Equal("Bearer", loginResponse.TokenType);
        Assert.Equal(3600, loginResponse.ExpiresIn);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "invalid",
            Password = "wrong"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyUsername_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_WithValidTokens_ReturnsNewToken()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "password123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var refreshRequest = new RefreshTokenRequestDto
        {
            AccessToken = loginResult!.AccessToken,
            RefreshToken = loginResult.RefreshToken
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var refreshResult = await response.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(refreshResult);
        Assert.NotEmpty(refreshResult.AccessToken);
        Assert.NotEmpty(refreshResult.RefreshToken);
        Assert.NotEqual(loginResult.RefreshToken, refreshResult.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidAccessToken_ReturnsUnauthorized()
    {
        // Arrange
        var refreshRequest = new RefreshTokenRequestDto
        {
            AccessToken = "invalid-token",
            RefreshToken = "some-refresh-token"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_WithEmptyRefreshToken_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "password123"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var refreshRequest = new RefreshTokenRequestDto
        {
            AccessToken = loginResult!.AccessToken,
            RefreshToken = ""
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/refresh", refreshRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsCorrectTokenStructure()
    {
        // Arrange
        var loginRequest = new LoginRequestDto
        {
            Username = "admin",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        Assert.NotNull(loginResponse);
        
        var tokenParts = loginResponse.AccessToken.Split('.');
        Assert.Equal(3, tokenParts.Length);
        
        Assert.True(loginResponse.ExpiresIn > 0);
        Assert.Equal("Bearer", loginResponse.TokenType);
    }
}

