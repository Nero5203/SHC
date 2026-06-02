namespace api.Dto.FileStorage
{
    public class UpdateFileMetadataDto
    {
        public string? FileName { get; set; }
        public Guid? FolderId { get; set; }
    }
}
