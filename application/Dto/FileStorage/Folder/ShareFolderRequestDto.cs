using domain.Entities.FileStorage.Enums;

namespace application.Dto.FileStorage.Folder
{
    public class ShareFolderRequestDto
    {
        public Guid FolderId { get; set; }
        public SharePermission Permission { get; set; }

        // Optional
        public DateTime? ExpiresAt { get; set; }
    }
}
