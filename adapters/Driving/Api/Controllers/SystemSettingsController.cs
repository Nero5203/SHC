using application.Dto.Settings;
using application.Ports.Driving.Settings;
using Domain.Entities.Settings;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/system-settings")]
    public class SystemSettingsController : ControllerBase
    {
        private readonly IGetSystemSettingsUseCase _getSystemSettingsUseCase;
        private readonly IListSystemSettingsUseCase _listSystemSettingsUseCase;
        private readonly IUpdateSystemSettingsUseCase _updateSystemSettingsUseCase;

        public SystemSettingsController(
            IGetSystemSettingsUseCase getSystemSettingsUseCase,
            IListSystemSettingsUseCase listSystemSettingsUseCase,
            IUpdateSystemSettingsUseCase updateSystemSettingsUseCase)
        {
            _getSystemSettingsUseCase = getSystemSettingsUseCase;
            _listSystemSettingsUseCase = listSystemSettingsUseCase;
            _updateSystemSettingsUseCase = updateSystemSettingsUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<SystemSettingResponseDto>> GetSystemSettings()
        {
            var systemSetting = await _getSystemSettingsUseCase.ExecuteAsync();

            return Ok(MapSystemSetting(systemSetting));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IReadOnlyList<SystemSettingResponseDto>>> ListSystemSettings()
        {
            var systemSettings = await _listSystemSettingsUseCase.ExecuteAsync();

            var response = systemSettings
                .Select(MapSystemSetting)
                .ToList();

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<SystemSettingResponseDto>> UpdateSystemSettings(UpdateSystemSettingDto dto)
        {
            try
            {
                var systemSetting = await _updateSystemSettingsUseCase.ExecuteAsync(
                    dto.MaxFileSizeInBytes,
                    dto.DefaultUserStorageQuotaInBytes,
                    dto.AllowedFileExtensions,
                    dto.AllowPublicLinkSharing,
                    dto.DefaultLinkExpirationInDays,
                    dto.EnforceLinkPasswordProtection,
                    dto.RegistrationMode,
                    dto.TwoFactorAuthRequired,
                    dto.MaxLoginAttempts,
                    dto.MinimumPasswordLength,
                    dto.RequireUppercasePassword,
                    dto.RequireNumberPassword,
                    dto.RequireSpecialCharacterPassword,
                    dto.TrashRetentionInDays);

                return Ok(MapSystemSetting(systemSetting));
            }
            catch (ArgumentException exception)
            {
                return BadRequest(exception.Message);
            }
        }

        private static SystemSettingResponseDto MapSystemSetting(SystemSetting systemSetting)
        {
            return new SystemSettingResponseDto
            {
                SystemSettingId = systemSetting.SystemSettingId,
                CreatedAt = systemSetting.CreatedAt,
                UpdatedAt = systemSetting.UpdatedAt,
                MaxFileSizeInBytes = systemSetting.MaxFileSizeInBytes,
                DefaultUserStorageQuotaInBytes = systemSetting.DefaultUserStorageQuotaInBytes,
                AllowedFileExtensions = systemSetting.AllowedFileExtensions,
                AllowPublicLinkSharing = systemSetting.AllowPublicLinkSharing,
                DefaultLinkExpirationInDays = systemSetting.DefaultLinkExpirationInDays,
                EnforceLinkPasswordProtection = systemSetting.EnforceLinkPasswordProtection,
                RegistrationMode = systemSetting.RegistrationMode,
                TwoFactorAuthRequired = systemSetting.TwoFactorAuthRequired,
                MaxLoginAttempts = systemSetting.MaxLoginAttempts,
                MinimumPasswordLength = systemSetting.MinimumPasswordLength,
                RequireUppercasePassword = systemSetting.RequireUppercasePassword,
                RequireNumberPassword = systemSetting.RequireNumberPassword,
                RequireSpecialCharacterPassword = systemSetting.RequireSpecialCharacterPassword,
                TrashRetentionInDays = systemSetting.TrashRetentionInDays
            };
        }
    }
}
