namespace application.Dto.Users
{
    public class UpdateUserSettingsDto
    {
        public string? Theme { get; set; }
        public string? Language { get; set; }
        public string? DefaultView { get; set; }

        public bool? EmailNotifications { get; set; }
        public bool? PushNotifications { get; set; }

        public bool? ShowProfilePicture { get; set; }
        public bool? ShowActivityStatus { get; set; }
    }
}