using application.Ports.Driven.Notifications;
using application.Ports.Driving.Notifications;

namespace application.UseCases.Notifications
{
    public class GetUnreadNotificationsByUserIdUseCase : IGetUnreadNotificationsByUserIdUseCase
    {
        private readonly INotificationRepository _notificationRepository;

        public GetUnreadNotificationsByUserIdUseCase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IReadOnlyList<global::Notification>> ExecuteAsync(Guid userId)
        {
            return await _notificationRepository.GetUnreadByUserIdAsync(userId);
        }
    }
}
