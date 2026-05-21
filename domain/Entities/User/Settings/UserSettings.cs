namespace Domain.Entities.User
{
    public class UserSettings
    {
        public UiSettings UiSettings { get; set; } = new ();
        public StorageSettings StorageSettings { get; set; } = new ();
        public NotificationSettings NotificationSettings { get; set; } = new ();
        public PrivacySettings PrivacySettings { get; set; } = new ();
    }
}