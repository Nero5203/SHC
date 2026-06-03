namespace api.Dto.FileStorage.File
{
    public class UploadFileRequestDto
    {
        public string FileName { get; set; } = null!;
        public Guid? FolderId { get; set; }
        public Stream Content { get; set; } = null!;
    }
}
