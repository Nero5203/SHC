using application.Ports.Driven.Auth;
using application.Ports.Driving.Auth;

namespace application.UseCases.Auth
{
    public class LogoutUserUseCase : ILogoutUserUseCase
    {

        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LogoutUserUseCase(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task LogoutUserAsync(LogoutRequest logoutRequest)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenAsync(logoutRequest.RefreshToken);
            if (refreshToken is not null)
            {
                await _refreshTokenRepository.DeleteRefreshTokenAsync(refreshToken);
            }
        }
    }
}