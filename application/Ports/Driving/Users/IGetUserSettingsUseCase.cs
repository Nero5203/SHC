using Domain.Entities.Users;

namespace application.Ports.Driving
{
    public interface IGetUserSettingsUseCase
    {
        Task<User?> ExecuteAsync(Guid userId);
    }
}
