using Domain.Entities.Users;

namespace ports.DrivingPorts
{
    public interface IGetUserSettingsUseCase
    {
        Task<User?> ExecuteAsync(Guid userId);
    }
}
