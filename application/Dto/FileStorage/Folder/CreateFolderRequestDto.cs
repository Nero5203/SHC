namespace application.Dto.FileStorage.Folder
{
    public class CreateFolderRequestDto
    {
        public string Name { get; set; } = null!;
        public Guid? ParentFolderId { get; set; }
    }
}
