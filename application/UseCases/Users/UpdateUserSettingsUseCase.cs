using Domain.Entities.Users;
using ports.DrivenPorts;
using ports.DrivingPorts;

namespace application.UseCases.Users
{
    public class UpdateUserSettingsUseCase : IUpdateUserSettingsUseCase
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserSettingsUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> ExecuteAsync(
            Guid userId,
            string? theme,
            string? language,
            string? defaultView,
            bool? emailNotifications,
            bool? pushNotifications,
            bool? showProfilePicture,
            bool? showActivityStatus)
        {
            var user = await _userRepository.GetWithSettingsByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (theme != null)
            {
                user.UserSettings.UiSettings.Theme = theme;
            }

            if (language != null)
            {
                user.UserSettings.UiSettings.Language = language;
            }

            if (defaultView != null)
            {
                user.UserSettings.UiSettings.DefaultView = defaultView;
            }

            if (emailNotifications.HasValue)
            {
                user.UserSettings.NotificationSettings.EmailNotifications = emailNotifications.Value;
            }

            if (pushNotifications.HasValue)
            {
                user.UserSettings.NotificationSettings.PushNotifications = pushNotifications.Value;
            }

            if (showProfilePicture.HasValue)
            {
                user.UserSettings.PrivacySettings.ShowProfilePicture = showProfilePicture.Value;
            }

            if (showActivityStatus.HasValue)
            {
                user.UserSettings.PrivacySettings.ShowActivityStatus = showActivityStatus.Value;
            }

            await _userRepository.UpdateAsync(user);

            return user;
        }
    }
}
