using application.Ports.Driven.Notifications;
using application.Ports.Driving.Notifications;

namespace application.UseCases.Notifications
{
    public class GetNotificationsByUserIdUseCase : IGetNotificationsByUserIdUseCase
    {
        private readonly INotificationRepository _notificationRepository;

        public GetNotificationsByUserIdUseCase(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IReadOnlyList<global::Notification>> ExecuteAsync(Guid userId)
        {
            return await _notificationRepository.GetByUserIdAsync(userId);
        }
    }
}
