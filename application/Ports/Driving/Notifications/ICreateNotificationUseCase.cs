using Domain.Entities.Notifications.Enums;

namespace application.Ports.Driving.Notifications
{
    public interface ICreateNotificationUseCase
    {
        Task<global::Notification> ExecuteAsync(
            string title,
            string message,
            NotificationType type,
            NotificationChannel channel,
            Guid userId,
            Guid? triggeredById,
            Guid? fileId,
            Guid? folderId);
    }
}
