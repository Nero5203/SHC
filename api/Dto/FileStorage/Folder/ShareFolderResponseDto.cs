namespace api.Dto.FileStorage.Folder
{
    public class ShareFolderResponseDto
    {
        public Guid ShareLinkId { get; set; }
        public string ShareUrl { get; set; } = null!;
        public DateTime? ExpiresAt { get; set; }
    }
}