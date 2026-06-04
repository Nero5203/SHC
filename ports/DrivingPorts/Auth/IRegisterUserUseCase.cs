using Application.Dtos.Auth;
namespace ports.DrivingPorts.Auth
{
    public interface IRegisterUserUseCase
    {
        Task RegisterUserAsync(RegisterDto registerDto);
    }
}