using application.Ports.Driven.Settings;
using application.Ports.Driving.Settings;
using Domain.Entities.Settings;
using Domain.Entities.Settings.Enums;

namespace application.UseCases.Settings
{
    public class UpdateSystemSettingsUseCase : IUpdateSystemSettingsUseCase
    {
        private readonly ISystemSettingRepository _systemSettingRepository;

        public UpdateSystemSettingsUseCase(ISystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
        }

        public async Task<SystemSetting> ExecuteAsync(
            long? maxFileSizeInBytes,
            long? defaultUserStorageQuotaInBytes,
            string? allowedFileExtensions,
            bool? allowPublicLinkSharing,
            int? defaultLinkExpirationInDays,
            bool? enforceLinkPasswordProtection,
            RegistrationMode? registrationMode,
            bool? twoFactorAuthRequired,
            int? maxLoginAttempts,
            int? minimumPasswordLength,
            bool? requireUppercasePassword,
            bool? requireNumberPassword,
            bool? requireSpecialCharacterPassword,
            int? trashRetentionInDays)
        {
            ValidatePositive(maxFileSizeInBytes, nameof(maxFileSizeInBytes));
            ValidatePositive(defaultUserStorageQuotaInBytes, nameof(defaultUserStorageQuotaInBytes));
            ValidatePositive(defaultLinkExpirationInDays, nameof(defaultLinkExpirationInDays));
            ValidatePositive(maxLoginAttempts, nameof(maxLoginAttempts));
            ValidatePositive(minimumPasswordLength, nameof(minimumPasswordLength));
            ValidatePositive(trashRetentionInDays, nameof(trashRetentionInDays));

            if (allowedFileExtensions != null && string.IsNullOrWhiteSpace(allowedFileExtensions))
            {
                throw new ArgumentException("Allowed file extensions cannot be empty.");
            }

            var systemSetting = await _systemSettingRepository.GetAsync();

            if (maxFileSizeInBytes.HasValue)
            {
                systemSetting.MaxFileSizeInBytes = maxFileSizeInBytes.Value;
            }

            if (defaultUserStorageQuotaInBytes.HasValue)
            {
                systemSetting.DefaultUserStorageQuotaInBytes = defaultUserStorageQuotaInBytes.Value;
            }

            if (allowedFileExtensions != null)
            {
                systemSetting.AllowedFileExtensions = allowedFileExtensions;
            }

            if (allowPublicLinkSharing.HasValue)
            {
                systemSetting.AllowPublicLinkSharing = allowPublicLinkSharing.Value;
            }

            if (defaultLinkExpirationInDays.HasValue)
            {
                systemSetting.DefaultLinkExpirationInDays = defaultLinkExpirationInDays.Value;
            }

            if (enforceLinkPasswordProtection.HasValue)
            {
                systemSetting.EnforceLinkPasswordProtection = enforceLinkPasswordProtection.Value;
            }

            if (registrationMode.HasValue)
            {
                systemSetting.RegistrationMode = registrationMode.Value;
            }

            if (twoFactorAuthRequired.HasValue)
            {
                systemSetting.TwoFactorAuthRequired = twoFactorAuthRequired.Value;
            }

            if (maxLoginAttempts.HasValue)
            {
                systemSetting.MaxLoginAttempts = maxLoginAttempts.Value;
            }

            if (minimumPasswordLength.HasValue)
            {
                systemSetting.MinimumPasswordLength = minimumPasswordLength.Value;
            }

            if (requireUppercasePassword.HasValue)
            {
                systemSetting.RequireUppercasePassword = requireUppercasePassword.Value;
            }

            if (requireNumberPassword.HasValue)
            {
                systemSetting.RequireNumberPassword = requireNumberPassword.Value;
            }

            if (requireSpecialCharacterPassword.HasValue)
            {
                systemSetting.RequireSpecialCharacterPassword = requireSpecialCharacterPassword.Value;
            }

            if (trashRetentionInDays.HasValue)
            {
                systemSetting.TrashRetentionInDays = trashRetentionInDays.Value;
            }

            systemSetting.UpdatedAt = DateTime.UtcNow;

            await _systemSettingRepository.UpdateAsync(systemSetting);

            return systemSetting;
        }

        private static void ValidatePositive(long? value, string propertyName)
        {
            if (value.HasValue && value.Value <= 0)
            {
                throw new ArgumentException($"{propertyName} must be greater than zero.");
            }
        }

        private static void ValidatePositive(int? value, string propertyName)
        {
            if (value.HasValue && value.Value <= 0)
            {
                throw new ArgumentException($"{propertyName} must be greater than zero.");
            }
        }
    }
}
