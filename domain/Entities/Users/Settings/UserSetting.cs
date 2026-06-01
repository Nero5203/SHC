using Domain.Entities.Users.Settings;

namespace Domain.Entities.Users
{
    public class UserSetting
    {
        public Guid UserSettingId { get; set; }
        public UiSetting UiSettings { get; set; } = new ();
        public StorageSetting StorageSettings { get; set; } = new ();
        public NotificationSetting NotificationSettings { get; set; } = new ();
        public PrivacySetting PrivacySettings { get; set; } = new ();
    }
}
