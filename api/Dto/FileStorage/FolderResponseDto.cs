namespace api.Dto.FileStorage
{
    public class FolderResponseDto
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; } = null!;
        public Guid? ParentFolderId { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
