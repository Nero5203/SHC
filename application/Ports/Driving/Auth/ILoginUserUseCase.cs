using application.Dto.Auth;
using application.UseCases.Auth;

namespace application.Ports.Driving.Auth
{
    public interface ILoginUserUseCase
    {
        Task <LoginResponseDto?>LoginUserAsync(LoginUserRequest request);
    }
}