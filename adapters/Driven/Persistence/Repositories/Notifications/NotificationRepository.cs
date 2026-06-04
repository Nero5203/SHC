using adapters.Driven.Persistence.Data;
using application.Ports.Driven.Notifications;
using Microsoft.EntityFrameworkCore;

namespace adapters.Driven.Persistence.Repositories.Notifications
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ShcDbContext _context;

        public NotificationRepository(ShcDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(global::Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<global::Notification?> GetByIdAsync(Guid notificationId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        }

        public async Task<IReadOnlyList<global::Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<global::Notification>> GetUnreadByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(global::Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<int> MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return notifications.Count;
        }

        public async Task DeleteAsync(global::Notification notification)
        {
            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }
    }
}
