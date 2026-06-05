namespace application.Dto.FileStorage.File
{
    public class SearchFilesRequestDto
    {
        public string? Query { get; set; }
        public Guid UserId { get; set; }
        public Guid? FolderId { get; set; }
    }
}
