using Domain.Entities.User;
using domain.Entities.LinkSharing.Enums;
public class SharedLink
{
    public Guid Id { get; set; }
    public string TokenUrl { get; set; } = null!;
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public bool CanView { get; set; } = true;
    public bool CanEdit { get; set; } = false;
    public bool AllowDownload { get; set; } = true;
   
    public Guid TargetId { get; set; }
    public ShareTargetType TargetType { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}