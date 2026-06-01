using Domain.Entities.Users.Settings;

namespace Domain.Entities.Users
{
    public class UserSetting
    {
        public Guid UserSettingId { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public UiSetting UiSettings { get; set; } = new ();
        public StorageSetting StorageSettings { get; set; } = new ();
        public NotificationSetting NotificationSettings { get; set; } = new ();
        public PrivacySetting PrivacySettings { get; set; } = new ();
    }
}
