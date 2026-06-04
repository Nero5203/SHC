using Domain.Entities.Users;

namespace application.Ports.Driving
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
