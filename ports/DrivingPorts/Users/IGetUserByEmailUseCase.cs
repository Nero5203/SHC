using Domain.Entities.Users;

namespace ports.DrivingPorts
{
    public interface IGetUserByEmailUseCase
    {
        Task<User?> ExecuteAsync(string email);
    }
}
