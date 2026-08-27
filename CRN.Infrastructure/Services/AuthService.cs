using CRN.Application.DTOs;
using CRN.Application.Interfaces;
using CRN.Domain.Entities;
using CRN.Infrastructure.Data;
using CRN.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CRN.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ApplicationDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        ApplicationDbContext dbContext,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request)
    {
        var existingUser = await _userManager.FindByNameAsync(
            request.UserName);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "Username is already registered.");
        }

        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }

        const string defaultRole = "User";

        if (!await _roleManager.RoleExistsAsync(defaultRole))
        {
            await _roleManager.CreateAsync(
                new IdentityRole(defaultRole));
        }

        await _userManager.AddToRoleAsync(
            user,
            defaultRole);

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponse?> LoginAsync(
        LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(
            request.UserName);

        if (user is null)
        {
            return null;
        }

        var validPassword = await _userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!validPassword)
        {
            return null;
        }

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponse?> RefreshTokenAsync(
        string refreshToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(
            refreshToken);

        var storedToken = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (storedToken is null ||
            storedToken.IsRevoked ||
            storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        var user = await _userManager.FindByIdAsync(
            storedToken.UserId);

        if (user is null)
        {
            return null;
        }

        // Rotate the refresh token.
        storedToken.RevokedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    private async Task<AuthResponse> GenerateTokensAsync(
        ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var tokenUser = new TokenUser
        {
            UserId = user.Id,
            UserName = user.UserName ?? string.Empty,
            Roles = roles
        };

        var (accessToken, expiresAt) =
            await _tokenService.CreateAccessTokenAsync(tokenUser);

        var refreshToken = _tokenService.CreateRefreshToken();

        var refreshTokenDays = int.Parse(
            _configuration["Jwt:RefreshTokenDays"] ?? "7");

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _tokenService.HashRefreshToken(
                refreshToken),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(
                refreshTokenDays)
        };

        await _dbContext.RefreshTokens.AddAsync(
            refreshTokenEntity);

        await _dbContext.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = expiresAt
        };
    }
}