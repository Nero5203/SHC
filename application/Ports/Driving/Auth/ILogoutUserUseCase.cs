using application.UseCases.Auth;

namespace application.Ports.Driving.Auth
{
    public interface ILogoutUserUseCase
    {
        Task LogoutUserAsync(LogoutRequest logoutRequest);
    }
}