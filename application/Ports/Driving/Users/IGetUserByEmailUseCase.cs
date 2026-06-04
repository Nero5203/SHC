using Domain.Entities.Users;

namespace application.Ports.Driving
{
    public interface IGetUserByEmailUseCase
    {
        Task<User?> ExecuteAsync(string email);
    }
}
