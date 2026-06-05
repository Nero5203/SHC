namespace application.Dto.FileStorage.File
{
    public class UploadFileRequestDto
    {
        public Guid UserId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public Guid? FolderId { get; set; }
        public Stream Content { get; set; } = null!;
    }
}
