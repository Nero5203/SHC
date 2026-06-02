namespace api.Dto.FileStorage
{
    public class FileItemResponseDto
    {
        public Guid FileItemId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileType { get; set; } = null!;
        public long FileSize { get; set; }
        public string Url { get; set; } = null!;
        public Guid? FolderId { get; set; }
        public Guid UserId { get; set; }
        public Guid StorageNodeId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
