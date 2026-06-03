namespace api.Dto.FileStorage.Folder
{
    public class RenameFolderRequestDto
    {
        public Guid FolderId { get; set; }
        public string NewName { get; set; } = null!;
    }
}
