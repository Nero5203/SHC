namespace api.Dto.Users
{
    public class UserSettingsResponseDto
    {
        public Guid UserSettingId { get; set; }
        public Guid UserId { get; set; }

        public string Theme { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string DefaultView { get; set; } = null!;

        public bool EmailNotifications { get; set; }
        public bool PushNotifications { get; set; }

        public bool ShowProfilePicture { get; set; }
        public bool ShowActivityStatus { get; set; }
    }
}