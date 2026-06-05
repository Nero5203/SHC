namespace application.Dto.FileStorage.Folder
{
    public class CreateFolderRequestDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public Guid? ParentFolderId { get; set; }
    }
}
