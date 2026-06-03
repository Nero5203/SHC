using Domain.Entities.Users;

namespace ports.DrivingPorts
{
    public interface IGetUserByIdUseCase
    {
        Task<User?> ExecuteAsync(Guid userId);
    }
}
