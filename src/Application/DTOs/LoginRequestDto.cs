namespace ProductAPI.Application.DTOs;

/// <summary>
/// Login request data transfer object
/// </summary>
public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

