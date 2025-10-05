namespace ProductAPI.Application.DTOs;

/// <summary>
/// Refresh token request data transfer object
/// </summary>
public class RefreshTokenRequestDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

