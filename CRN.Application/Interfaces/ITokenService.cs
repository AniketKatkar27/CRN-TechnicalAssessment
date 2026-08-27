using CRN.Application.DTOs;

namespace CRN.Application.Interfaces;

public interface ITokenService
{
    Task<(string Token, DateTime ExpiresAt)> CreateAccessTokenAsync(
        TokenUser user);

    string CreateRefreshToken();

    string HashRefreshToken(string refreshToken);
}