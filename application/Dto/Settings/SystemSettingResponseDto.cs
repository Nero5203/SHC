using Domain.Entities.Settings.Enums;

namespace application.Dto.Settings
{
    public class SystemSettingResponseDto
    {
        public Guid SystemSettingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long MaxFileSizeInBytes { get; set; }
        public long DefaultUserStorageQuotaInBytes { get; set; }
        public string AllowedFileExtensions { get; set; } = null!;
        public bool AllowPublicLinkSharing { get; set; }
        public int DefaultLinkExpirationInDays { get; set; }
        public bool EnforceLinkPasswordProtection { get; set; }
        public RegistrationMode RegistrationMode { get; set; }
        public bool TwoFactorAuthRequired { get; set; }
        public int MaxLoginAttempts { get; set; }
        public int MinimumPasswordLength { get; set; }
        public bool RequireUppercasePassword { get; set; }
        public bool RequireNumberPassword { get; set; }
        public bool RequireSpecialCharacterPassword { get; set; }
        public int TrashRetentionInDays { get; set; }
    }
}
