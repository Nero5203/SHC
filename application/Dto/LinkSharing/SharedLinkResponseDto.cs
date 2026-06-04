using Domain.Entities.LinkSharing.Enums;

namespace application.Dto.LinkSharing
{
    public class SharedLinkResponseDto
    {
        public Guid SharedLinkId { get; set; }
        public string TokenUrl { get; set; } = null!;

        public Guid UserId { get; set; }
        public Guid TargetId { get; set; }
        public ShareTargetType TargetType { get; set; }

        public DateTime? ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool AllowDownload { get; set; }
    }
}
