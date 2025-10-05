using System.Security.Claims;

namespace ProductAPI.Application.Interfaces;

/// <summary>
/// Interface for JWT token management
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified username
    /// </summary>
    /// <param name="username">Username to generate token for</param>
    /// <returns>JWT token string</returns>
    string GenerateAccessToken(string username);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <returns>Refresh token string</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates and extracts claims from an expired token
    /// </summary>
    /// <param name="token">The expired access token</param>
    /// <returns>Claims principal if valid, null otherwise</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

