using System;
using System.Collections.Generic;
using domain.Entities.Settings.Enums;

namespace Domain.Entities.Settings
{
   
    public class SystemSettings
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Auditing
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Storage & Upload Limits
        public long MaxFileSizeInBytes { get; set; } = 1073741824; // 1 GB

        public long DefaultUserStorageQuotaInBytes { get; set; }
            = 10737418240; // 10 GB

        public string AllowedFileExtensions { get; set; }
     = ".jpg,.jpeg,.png,.gif,.pdf,.docx,.xlsx,.zip,.mp4";

        // Link Sharing
        public bool AllowPublicLinkSharing { get; set; } = true;

        public int DefaultLinkExpirationInDays { get; set; } = 7;

        public bool EnforceLinkPasswordProtection { get; set; } = false;

        // Authentication & Security
        public RegistrationMode RegistrationMode { get; set; }
            = RegistrationMode.Enabled;

        public bool TwoFactorAuthRequired { get; set; } = false;

        public int MaxLoginAttempts { get; set; } = 5;

        public int MinimumPasswordLength { get; set; } = 8;

        public bool RequireUppercasePassword { get; set; } = true;

        public bool RequireNumberPassword { get; set; } = true;

        public bool RequireSpecialCharacterPassword { get; set; } = true;

        // Maintenance
        public int TrashRetentionInDays { get; set; } = 30;
    }
}