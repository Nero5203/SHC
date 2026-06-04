namespace application.Ports.Driving.Notifications
{
    public interface IDeleteNotificationUseCase
    {
        Task<bool> ExecuteAsync(Guid notificationId);
    }
}
