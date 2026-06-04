using application.Ports.Driven.Notifications;
using application.Ports.Driving.Notifications;

namespace application.UseCases.Notifications
{
    public class DeleteNotificationUseCase : IDeleteNotificationUseCase
    {
        private readonly INotificationRepository _notificationRepository;

        public DeleteNotificationUseCase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<bool> ExecuteAsync(Guid notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);

            if (notification == null)
            {
                return false;
            }

            await _notificationRepository.DeleteAsync(notification);

            return true;
        }
    }
}
