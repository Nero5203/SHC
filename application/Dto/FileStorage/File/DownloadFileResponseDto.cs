namespace api.Dto.FileStorage.File
{
    public class DownloadFileResponseDto
    {
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public Stream Content { get; set; } = null!;
    }
}
