using application.Ports.Driven.Notifications;
using application.Ports.Driving.Notifications;

namespace application.UseCases.Notifications
{
    public class MarkNotificationAsReadUseCase : IMarkNotificationAsReadUseCase
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkNotificationAsReadUseCase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<global::Notification?> ExecuteAsync(Guid notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return null;
            }

            notification.IsRead = true;

            await _notificationRepository.UpdateAsync(notification);

            return notification;
        }
    }
}
