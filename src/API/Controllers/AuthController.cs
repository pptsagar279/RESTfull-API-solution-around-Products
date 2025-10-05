using Microsoft.AspNetCore.Mvc;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.Interfaces;

namespace ProductAPI.API.Controllers;

/// <summary>
/// Authentication controller for JWT token management
/// </summary>
[ApiController]
[Route("api/v1/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAuthenticationService authenticationService,
        ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    /// <summary>
    /// Login endpoint to get JWT token
    /// </summary>
    /// <param name="loginRequest">Login credentials</param>
    /// <returns>JWT token and refresh token</returns>
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
    {
        var result = await _authenticationService.LoginAsync(loginRequest);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Refresh token endpoint
    /// </summary>
    /// <param name="refreshRequest">Refresh token request</param>
    /// <returns>New JWT token</returns>
    [HttpPost]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshRequest)
    {
        var result = await _authenticationService.RefreshTokenAsync(refreshRequest);

        if (result == null)
        {
            return Unauthorized(new { message = "Invalid token" });
        }

        return Ok(result);
    }
}
