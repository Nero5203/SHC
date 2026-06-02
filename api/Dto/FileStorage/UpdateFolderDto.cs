namespace api.Dto.FileStorage
{
    public class UpdateFolderDto
    {
        public string? Name { get; set; }
        public Guid? ParentFolderId { get; set; }
    }
}
