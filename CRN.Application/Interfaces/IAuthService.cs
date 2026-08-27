using CRN.Application.DTOs;

namespace CRN.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);

    Task<AuthResponse?> LoginAsync(LoginRequest request);

    Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
}