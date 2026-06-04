namespace application.Ports.Driving.Notifications
{
    public interface IMarkAllNotificationsAsReadUseCase
    {
        Task<int> ExecuteAsync(Guid userId);
    }
}
