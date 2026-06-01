using Domain.Entities.AI;
using Domain.Entities.Auth;
using Domain.Entities.LinkSharing;
using Domain.Entities.Purchases;
using SHC.Domain.Entities.Permissions;

namespace Domain.Entities.Users
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        
        public string? ProfilePictureUrl { get; set; }
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserCredential UserCredentials { get; set; } = null!;
        public UserSetting Settings { get; set; } = new ();


        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<SharedLink> SharedLinks { get; set; } = new List<SharedLink>();
        public ICollection<FileStorage.FileItem> FileItems { get; set; } = new List<FileStorage.FileItem>();
        public ICollection<FileStorage.Folder> Folders { get; set; } = new List<FileStorage.Folder>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
        public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
        public ICollection<AISuggestion> AISuggestions { get; set; } = new List<AISuggestion>();
    }
}
