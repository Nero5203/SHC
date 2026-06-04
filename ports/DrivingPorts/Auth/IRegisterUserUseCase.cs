using application.Dto.Auth;
namespace ports.DrivingPorts.Auth
{
    public interface IRegisterUserUseCase
    {
        Task<string?> ExecuteAsync(RegisterDto registerDto);
    }
}