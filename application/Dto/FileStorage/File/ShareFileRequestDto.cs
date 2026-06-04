using domain.Entities.FileStorage.Enums;

namespace application.Dto.FileStorage.File
{
    public class ShareFileRequestDto
    {
        public Guid FileItemId { get; set; }
        public SharePermission Permission { get; set; }

        // Optional
        public DateTime? ExpiresAt { get; set; }
    }
}
