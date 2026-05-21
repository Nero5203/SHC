namespace Domain.Entities.User.Settings
{
    public class NotificationSettings
    {
        public bool EmailNotifications { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        public bool SmsNotifications { get; set; } = false;
        public bool InAppNotifications { get; set; } = true;
        public bool FileShared { get; set; } = true;
        public bool StorageLow { get; set; } = true;

    }
}