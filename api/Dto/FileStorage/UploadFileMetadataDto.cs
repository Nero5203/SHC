namespace api.Dto.FileStorage
{
    public class UploadFileMetadataDto
    {
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public string Url { get; set; } = null!;
        public Guid? FolderId { get; set; }
        public Guid StorageNodeId { get; set; }
    }
}
