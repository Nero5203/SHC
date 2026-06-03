namespace api.Dto.FileStorage.Folder
{
    public class FolderDto
    {
        public Guid FolderId { get; set; }
        public string Name { get; set; } = null!;
        public Guid? ParentFolderId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
