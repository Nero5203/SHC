using Domain.Entities.Users;
using ports.DrivenPorts;

namespace application.UseCases.Users
{
    public class UserUseCase : IUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public UserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User?> GetUserSettingsAsync(Guid userId)
        {
            return await _userRepository.GetWithSettingsByIdAsync(userId);
        }

        public async Task<User?> UpdateUserProfileAsync(
            Guid userId,
            string? username,
            string? firstName,
            string? lastName,
            string? profilePictureUrl,
            string? phoneNumber)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            if (username != null)
            {
                user.Username = username;
            }

            if (firstName != null)
            {
                user.FirstName = firstName;
            }

            if (lastName != null)
            {
                user.LastName = lastName;
            }

            if (profilePictureUrl != null)
            {
                user.ProfilePictureUrl = profilePictureUrl;
            }

            if (phoneNumber != null)
            {
                user.PhoneNumber = phoneNumber;
            }

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return user;
        }

        public async Task<User?> UpdateUserSettingsAsync(
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

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(user);

            return true;
        }
    }
}