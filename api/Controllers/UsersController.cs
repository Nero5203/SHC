using api.Dto.Users;
using Microsoft.AspNetCore.Mvc;
using ports.DrivingPorts;

namespace api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IGetUserByIdUseCase _getUserByIdUseCase;
        private readonly IGetUserSettingsUseCase _getUserSettingsUseCase;
        private readonly IUpdateUserProfileUseCase _updateUserProfileUseCase;
        private readonly IUpdateUserSettingsUseCase _updateUserSettingsUseCase;
        private readonly IDeleteUserUseCase _deleteUserUseCase;

        public UsersController(
            IGetUserByIdUseCase getUserByIdUseCase,
            IGetUserSettingsUseCase getUserSettingsUseCase,
            IUpdateUserProfileUseCase updateUserProfileUseCase,
            IUpdateUserSettingsUseCase updateUserSettingsUseCase,
            IDeleteUserUseCase deleteUserUseCase)
        {
            _getUserByIdUseCase = getUserByIdUseCase;
            _getUserSettingsUseCase = getUserSettingsUseCase;
            _updateUserProfileUseCase = updateUserProfileUseCase;
            _updateUserSettingsUseCase = updateUserSettingsUseCase;
            _deleteUserUseCase = deleteUserUseCase;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(Guid userId)
        {
            var user = await _getUserByIdUseCase.ExecuteAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUserProfile(
            Guid userId,
            UpdateUserDto dto)
        {
            var user = await _updateUserProfileUseCase.ExecuteAsync(
                userId,
                dto.Username,
                dto.FirstName,
                dto.LastName,
                dto.ProfilePictureUrl,
                dto.PhoneNumber);

            if (user == null)
            {
                return NotFound();
            }

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePictureUrl = user.ProfilePictureUrl,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }

        [HttpGet("{userId}/settings")]
        public async Task<ActionResult<UserSettingsResponseDto>> GetUserSettings(Guid userId)
        {
            var user = await _getUserSettingsUseCase.ExecuteAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var settings = user.UserSettings;

            var response = new UserSettingsResponseDto
            {
                UserSettingId = settings.UserSettingId,
                UserId = settings.UserId,

                Theme = settings.UiSettings.Theme,
                Language = settings.UiSettings.Language,
                DefaultView = settings.UiSettings.DefaultView,

                EmailNotifications = settings.NotificationSettings.EmailNotifications,
                PushNotifications = settings.NotificationSettings.PushNotifications,

                ShowProfilePicture = settings.PrivacySettings.ShowProfilePicture,
                ShowActivityStatus = settings.PrivacySettings.ShowActivityStatus
            };

            return Ok(response);
        }

        [HttpPut("{userId}/settings")]
        public async Task<ActionResult<UserSettingsResponseDto>> UpdateUserSettings(
            Guid userId,
            UpdateUserSettingsDto dto)
        {
            var user = await _updateUserSettingsUseCase.ExecuteAsync(
                userId,
                dto.Theme,
                dto.Language,
                dto.DefaultView,
                dto.EmailNotifications,
                dto.PushNotifications,
                dto.ShowProfilePicture,
                dto.ShowActivityStatus);

            if (user == null)
            {
                return NotFound();
            }

            var settings = user.UserSettings;

            var response = new UserSettingsResponseDto
            {
                UserSettingId = settings.UserSettingId,
                UserId = settings.UserId,

                Theme = settings.UiSettings.Theme,
                Language = settings.UiSettings.Language,
                DefaultView = settings.UiSettings.DefaultView,

                EmailNotifications = settings.NotificationSettings.EmailNotifications,
                PushNotifications = settings.NotificationSettings.PushNotifications,

                ShowProfilePicture = settings.PrivacySettings.ShowProfilePicture,
                ShowActivityStatus = settings.PrivacySettings.ShowActivityStatus
            };

            return Ok(response);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var deleted = await _deleteUserUseCase.ExecuteAsync(userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
