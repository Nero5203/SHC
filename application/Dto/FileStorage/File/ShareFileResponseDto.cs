namespace api.Dto.FileStorage.File
{
    public class ShareFileResponseDto
    {
        public Guid ShareLinkId { get; set; }
        public string ShareUrl { get; set; } = null!;
        public DateTime? ExpiresAt { get; set; }
    }
}
