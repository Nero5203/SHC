using Domain.Entities.Users;

namespace application.Ports.Driving
{
    public interface IGetUserByIdUseCase
    {
        Task<User?> ExecuteAsync(Guid userId);
    }
}
