using application.Ports.Driven.Notifications;
using application.Ports.Driving.Notifications;

namespace application.UseCases.Notifications
{
    public class MarkAllNotificationsAsReadUseCase : IMarkAllNotificationsAsReadUseCase
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkAllNotificationsAsReadUseCase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<int> ExecuteAsync(Guid userId)
        {
            return await _notificationRepository.MarkAllAsReadAsync(userId);
        }
    }
}
