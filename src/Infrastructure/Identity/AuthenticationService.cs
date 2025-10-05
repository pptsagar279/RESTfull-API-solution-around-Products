using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;

namespace ProductAPI.Infrastructure.Identity;

/// <summary>
/// Service for handling authentication operations
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly ITokenService _tokenService;
    private readonly IAppLogger<AuthenticationService> _logger;

    public AuthenticationService(
        ITokenService tokenService,
        IAppLogger<AuthenticationService> logger)
    {
        _tokenService = tokenService;
        _logger = logger;
    }

    public Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
    {
        _logger.LogInformation("Login attempt for user: {Username}", loginRequest.Username);

        if (IsValidUser(loginRequest.Username, loginRequest.Password))
        {
            var accessToken = _tokenService.GenerateAccessToken(loginRequest.Username);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var response = new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 3600, // 1 hour
                TokenType = "Bearer"
            };

            _logger.LogInformation("Login successful for user: {Username}", loginRequest.Username);
            return Task.FromResult<LoginResponseDto?>(response);
        }

        _logger.LogWarning("Login failed for user: {Username}", loginRequest.Username);
        return Task.FromResult<LoginResponseDto?>(null);
    }

    public Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshRequest)
    {
        _logger.LogInformation("Token refresh attempt");

        if (IsValidRefreshToken(refreshRequest.RefreshToken))
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(refreshRequest.AccessToken);
            if (principal?.Identity?.Name != null)
            {
                var newAccessToken = _tokenService.GenerateAccessToken(principal.Identity.Name);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                var response = new LoginResponseDto
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresIn = 3600,
                    TokenType = "Bearer"
                };

                _logger.LogInformation("Token refresh successful");
                return Task.FromResult<LoginResponseDto?>(response);
            }
        }

        _logger.LogWarning("Token refresh failed");
        return Task.FromResult<LoginResponseDto?>(null);
    }

    private bool IsValidUser(string username, string password)
    {
        var validUsers = new Dictionary<string, string>
        {
            { "admin", "password123" },
            { "manager", "password123" },
            { "user", "password123" },
            { "readonly", "password123" }
        };
        
        return validUsers.TryGetValue(username.ToLower(), out var validPassword) && 
               validPassword == password;
    }

    private bool IsValidRefreshToken(string refreshToken)
    {
        return !string.IsNullOrEmpty(refreshToken);
    }
}

