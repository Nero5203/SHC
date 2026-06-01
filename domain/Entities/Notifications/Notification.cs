using Domain.Entities.FileStorage;
using Domain.Entities.Notifications.Enums;
using Domain.Entities.Users;

public class Notification
{
    public Guid NotificationId { get; set; }
    public string Title { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public NotificationType Type { get; set; }
    public NotificationChannel Channel { get; set; }

    // The recipient of the notification
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // (the person sharing a file)
    public Guid? TriggeredById { get; set; }
    public User? TriggeredBy { get; set; }

   
    public Guid? FileId { get; set; }
    public FileItem? FileItems { get; set; }


    public Guid? FolderId { get; set; }
    public Folder? Folder { get; set; }
}