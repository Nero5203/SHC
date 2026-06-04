namespace application.Dto.FileStorage.File
{
    public class SearchFilesRequestDto
    {
        public string Query { get; set; } = null!;
        public Guid? FolderId { get; set; }
    }
}
