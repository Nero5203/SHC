namespace Domain.Entities.User.Settings
{
    public class PrivacySettings
    {
        public bool AllowLinkDownload { get; set; } = true;
        public bool ShowActivityStatus { get; set; } = true;
        public bool ShowProfilePicture { get; set; } = true;
        public bool ShowRecentlyUsedFiles { get; set; } = true;
        public bool AllowSearchEngineIndexing { get; set; } = true;
    }
}