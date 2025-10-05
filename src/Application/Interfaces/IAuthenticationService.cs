using ProductAPI.Application.DTOs;

namespace ProductAPI.Application.Interfaces;

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user and returns JWT tokens
    /// </summary>
    /// <param name="loginRequest">Login credentials</param>
    /// <returns>Login response with access and refresh tokens</returns>
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);

    /// <summary>
    /// Refreshes an expired access token
    /// </summary>
    /// <param name="refreshRequest">Refresh token request</param>
    /// <returns>New access and refresh tokens</returns>
    Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshRequest);
}

