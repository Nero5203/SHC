namespace application.Dto.FileStorage.Folder
{
    public class MoveFolderRequestDto
    {
        public Guid FolderId { get; set; }
        public Guid? TargetParentFolderId { get; set; }
    }
}
