using application.UseCases.Auth;

namespace application.Ports.Driving.Auth
{
    public interface IRegisterUserUseCase
    {
        Task RegisterUserAsync(RegisterUserRequest registerUserRequest);
    }
}
