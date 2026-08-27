using CRN.Application.DTOs;
using CRN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRN.API.Controllers;

/// <summary>
/// Provides endpoints for user registration, authentication, and token management.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The user registration details.</param>
    /// <returns>An authentication response for the registered user.</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        return Ok(response);
    }

    /// <summary>
    /// Authenticates a user and generates access and refresh tokens.
    /// </summary>
    /// <param name="request">The user's login credentials.</param>
    /// <returns>An authentication response containing the tokens.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(
        LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response is null)
        {
            return Unauthorized(new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Invalid username or password."
            });
        }

        return Ok(response);
    }

    /// <summary>
    /// Generates a new access token using a valid refresh token.
    /// </summary>
    /// <param name="request">The refresh token request.</param>
    /// <returns>A new authentication response.</returns>
    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(
        RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(
            request.RefreshToken);

        if (response is null)
        {
            return Unauthorized(new ErrorResponse
            {
                StatusCode = StatusCodes.Status401Unauthorized,
                Message = "Invalid or expired refresh token."
            });
        }

        return Ok(response);
    }
}