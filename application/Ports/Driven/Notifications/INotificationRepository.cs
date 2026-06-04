namespace application.Ports.Driven.Notifications
{
    public interface INotificationRepository
    {
        Task CreateAsync(global::Notification notification);
        Task<global::Notification?> GetByIdAsync(Guid notificationId);
        Task<IReadOnlyList<global::Notification>> GetByUserIdAsync(Guid userId);
        Task<IReadOnlyList<global::Notification>> GetUnreadByUserIdAsync(Guid userId);
        Task UpdateAsync(global::Notification notification);
        Task<int> MarkAllAsReadAsync(Guid userId);
        Task DeleteAsync(global::Notification notification);
    }
}
