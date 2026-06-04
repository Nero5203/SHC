namespace application.Dto.FileStorage.File
{
    public class FileDto
    {
        public Guid FileItemId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public string Url { get; set; } = null!;
        public Guid? FolderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
