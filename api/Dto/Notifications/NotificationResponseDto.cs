using Domain.Entities.Notifications.Enums;

namespace api.Dto.Notifications
{
    public class NotificationResponseDto
    {
        public Guid NotificationId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public Guid UserId { get; set; }
        public Guid? TriggeredById { get; set; }
        public Guid? FileId { get; set; }
        public Guid? FolderId { get; set; }
    }
}
