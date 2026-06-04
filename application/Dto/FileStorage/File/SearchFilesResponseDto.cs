namespace application.Dto.FileStorage.File
{
    public class SearchFilesResponseDto
    {
        public IEnumerable<FileDto> Files { get; set; } =  Enumerable.Empty<FileDto>();
    }
}
