using application.UseCases.Auth;

namespace application.Ports.Driving.Auth
{
    public interface ILoginUserUseCase
    {
        Task <string?>LoginUserAsync(LoginUserRequest request);
    }
}