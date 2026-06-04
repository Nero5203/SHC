namespace application.Ports.Driving.Notifications
{
    public interface IGetUnreadNotificationsByUserIdUseCase
    {
        Task<IReadOnlyList<global::Notification>> ExecuteAsync(Guid userId);
    }
}
