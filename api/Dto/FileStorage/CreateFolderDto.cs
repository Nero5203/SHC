namespace api.Dto.FileStorage
{
    public class CreateFolderDto
    {
        public string Name { get; set; } = null!;
        public Guid? ParentFolderId { get; set; }
    }
}
