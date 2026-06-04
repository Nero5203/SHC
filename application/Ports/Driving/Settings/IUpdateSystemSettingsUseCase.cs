using Domain.Entities.Settings;
using Domain.Entities.Settings.Enums;

namespace application.Ports.Driving.Settings
{
    public interface IUpdateSystemSettingsUseCase
    {
        Task<SystemSetting> ExecuteAsync(
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
            int? trashRetentionInDays);
    }
}
