using application.Ports.Driven.Notifications;
using application.Ports.Driving.Notifications;
using Domain.Entities.Notifications.Enums;

namespace application.UseCases.Notifications
{
    public class CreateNotificationUseCase : ICreateNotificationUseCase
    {
        private readonly INotificationRepository _notificationRepository;

        public CreateNotificationUseCase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<global::Notification> ExecuteAsync(
            string title,
            string message,
            NotificationType type,
            NotificationChannel channel,
            Guid userId,
            Guid? triggeredById,
            Guid? fileId,
            Guid? folderId)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Notification title is required.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Notification message is required.");
            }

            var notification = new global::Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Type = type,
                Channel = channel,
                UserId = userId,
                TriggeredById = triggeredById,
                FileId = fileId,
                FolderId = folderId
            };

            await _notificationRepository.CreateAsync(notification);

            return notification;
        }
    }
}
