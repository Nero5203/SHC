namespace application.Ports.Driving.Notifications
{
    public interface IGetNotificationByIdUseCase
    {
        Task<global::Notification?> ExecuteAsync(Guid notificationId);
    }
}
