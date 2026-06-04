using Domain.Entities.Auth;

namespace application.Ports.Driven.Auth
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task AddRefreshTokenAsync(RefreshToken refreshToken);
        Task DeleteRefreshTokenAsync(RefreshToken refreshToken);
    }
}