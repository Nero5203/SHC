using Domain.Entities.Users;

namespace ports.DrivingPorts.Users
{
    public interface IUpdateUserSettingsUseCase
    {
        Task<User?> ExecuteAsync(
            Guid userId,
            string? theme,
            string? language,
            string? defaultView,
            bool? emailNotifications,
            bool? pushNotifications,
            bool? showProfilePicture,
            bool? showActivityStatus);
    }
}
