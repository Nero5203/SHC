using Domain.Entities.Notifications.Enums;

namespace application.Dto.Notifications
{
    public class CreateNotificationDto
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public NotificationType Type { get; set; }
        public NotificationChannel Channel { get; set; }
        public Guid UserId { get; set; }
        public Guid? TriggeredById { get; set; }
        public Guid? FileId { get; set; }
        public Guid? FolderId { get; set; }
    }
}
