namespace application.Dto.FileStorage.File
{
    public class MoveFileRequestDto
    {
        public Guid FileItemId { get; set; }
        public Guid? TargetFolderId { get; set; }
    }
}
