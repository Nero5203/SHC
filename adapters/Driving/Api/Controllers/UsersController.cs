using application.Common.Authorization;
using application.Dto.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using application.Ports.Driving;
using application.Ports.Driving.Auth;

namespace api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IGetAllUsersUseCase _getAllUsersUseCase;
        private readonly IGetUserByIdUseCase _getUserByIdUseCase;
        private readonly IGetUserByEmailUseCase _getUserByEmailUseCase;
        private readonly IGetUserSettingsUseCase _getUserSettingsUseCase;
        private readonly IUpdateUserProfileUseCase _updateUserProfileUseCase;
        private readonly IUpdateUserSettingsUseCase _updateUserSettingsUseCase;
        private readonly IDeleteUserUseCase _deleteUserUseCase;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserAuthorizationService _userAuthorizationService;

        public UsersController(
            IGetAllUsersUseCase getAllUsersUseCase,
            IGetUserByIdUseCase getUserByIdUseCase,
            IGetUserByEmailUseCase getUserByEmailUseCase,
            IGetUserSettingsUseCase getUserSettingsUseCase,
            IUpdateUserProfileUseCase updateUserProfileUseCase,
            IUpdateUserSettingsUseCase updateUserSettingsUseCase,
            IDeleteUserUseCase deleteUserUseCase,
            ICurrentUserService currentUserService,
            IUserAuthorizationService userAuthorizationService)
        {
            _getAllUsersUseCase = getAllUsersUseCase;
            _getUserByIdUseCase = getUserByIdUseCase;
            _getUserByEmailUseCase = getUserByEmailUseCase;
            _getUserSettingsUseCase = getUserSettingsUseCase;
            _updateUserProfileUseCase = updateUserProfileUseCase;
            _updateUserSettingsUseCase = updateUserSettingsUseCase;
            _deleteUserUseCase = deleteUserUseCase;
            _currentUserService = currentUserService;
            _userAuthorizationService = userAuthorizationService;
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.Admin)]
        public async Task<ActionResult<IReadOnlyList<UserResponseDto>>> GetAllUsers()
        {
            var users = await _getAllUsersUseCase.ExecuteAsync();

            var response = users
                .Select(MapUser)
                .ToList();

            return Ok(response);
        }

        [HttpGet("by-email")]
        public async Task<ActionResult<UserResponseDto>> GetUserByEmail([FromQuery] string email)
        {
            var user = await _getUserByEmailUseCase.ExecuteAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(MapUser(user));
        }

        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(Guid userId)
        {
            if (!CanAccessUser(userId))
            {
                return Forbid();
            }

            var user = await _getUserByIdUseCase.ExecuteAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(MapUser(user));
        }

        [HttpPut("{userId}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUserProfile(
            Guid userId,
            UpdateUserDto dto)
        {
            if (!CanAccessUser(userId))
            {
                return Forbid();
            }

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

            return Ok(MapUser(user));
        }

        [HttpGet("{userId}/settings")]
        public async Task<ActionResult<UserSettingsResponseDto>> GetUserSettings(Guid userId)
        {
            if (!CanAccessUser(userId))
            {
                return Forbid();
            }

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
            if (!CanAccessUser(userId))
            {
                return Forbid();
            }

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
        [Authorize(Roles = AuthorizationRoles.Admin)]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var deleted = await _deleteUserUseCase.ExecuteAsync(userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private static UserResponseDto MapUser(Domain.Entities.Users.User user)
        {
            return new UserResponseDto
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
        }

        private bool CanAccessUser(Guid userId)
        {
            return _userAuthorizationService.IsAdmin() || _currentUserService.UserId == userId;
        }
    }
}
