namespace api.Dto.LinkSharing
{
    public class UpdateSharedLinkDto
    {
        public DateTime? ExpirationDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? CanView { get; set; }
        public bool? CanEdit { get; set; }
        public bool? AllowDownload { get; set; }
    }
}
