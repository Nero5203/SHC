namespace api.Dto.FileStorage.File
{
    public class RenameFileRequestDto
    {
        public Guid FileItemId { get; set; }
        public string NewName { get; set; } = null!;
    }
}
