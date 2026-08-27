using CRN.Application.DTOs;
using CRN.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRN.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(
        RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        return Ok(response);
    }

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