using application.Dto.FileStorage.File;

namespace application.Dto.FileStorage.Folder
{
    public class ListFolderContentsResponseDto
    {
        public IEnumerable<FolderDto> Folders { get; set; }
       = Enumerable.Empty<FolderDto>();

        public IEnumerable<FileDto> Files { get; set; }
            = Enumerable.Empty<FileDto>();
    }
}
