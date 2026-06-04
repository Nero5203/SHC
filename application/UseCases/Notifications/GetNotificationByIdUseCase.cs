using application.Ports.Driven.Notifications;
using application.Ports.Driving.Notifications;

namespace application.UseCases.Notifications
{
    public class GetNotificationByIdUseCase : IGetNotificationByIdUseCase
    {
        private readonly INotificationRepository _notificationRepository;

        public GetNotificationByIdUseCase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<global::Notification?> ExecuteAsync(Guid notificationId)
        {
            return await _notificationRepository.GetByIdAsync(notificationId);
        }
    }
}
