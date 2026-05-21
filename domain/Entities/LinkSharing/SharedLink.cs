using Domain.Entities.FileStorage;
using Domain.Entities.Users;
using Domain.Entities.LinkSharing.Enums;


namespace Domain.Entities.LinkSharing
{

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
        public Guid? FileId { get; set; }
        public FileItem? File { get; set; }

        public Guid? FolderId { get; set; }
        public Folder? Folder { get; set; }

    }
}
