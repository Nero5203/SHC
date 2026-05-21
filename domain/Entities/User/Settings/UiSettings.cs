namespace Domain.Entities.User.Settings
{
    public class UiSettings
    {
        public string Theme { get; set; } = "light";
        public string Language { get; set; } = "en";

        public string DefaultView { get; set; } = "list";

        public string SortBy { get; set; } = "dateCreated";
        public bool ShowHiddenFiles { get; set; } = false;
    }
}