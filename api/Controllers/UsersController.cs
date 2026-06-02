using api.Dto.Users;
using application.UseCases.Users;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserUseCase _userUseCase;

        public UsersController(IUserUseCase userUseCase)
        {
            _userUseCase = userUseCase;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(Guid userId)
        {
            var user = await _userUseCase.GetUserByIdAsync(userId);

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
            var user = await _userUseCase.UpdateUserProfileAsync(
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
            var user = await _userUseCase.GetUserSettingsAsync(userId);

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
            var user = await _userUseCase.UpdateUserSettingsAsync(
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
            var deleted = await _userUseCase.DeleteUserAsync(userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}