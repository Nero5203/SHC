using Domain.Entities.LinkSharing.Enums;

namespace api.Dto.LinkSharing
{
    public class CreateSharedLinkDto
    {
        public Guid TargetId { get; set; }
        public ShareTargetType TargetType { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanEdit { get; set; }
        public bool AllowDownload { get; set; } = true;
    }
}
