namespace application.Ports.Driving.Notifications
{
    public interface IMarkNotificationAsReadUseCase
    {
        Task<global::Notification?> ExecuteAsync(Guid notificationId);
    }
}
