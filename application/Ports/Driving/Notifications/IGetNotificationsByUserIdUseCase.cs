namespace application.Ports.Driving.Notifications
{
    public interface IGetNotificationsByUserIdUseCase
    {
        Task<IReadOnlyList<global::Notification>> ExecuteAsync(Guid userId);
    }
}
