using domain.Entities.FileStorage;
using Domain.Entities.AI;
using Domain.Entities.Auth;
using Domain.Entities.LinkSharing;
using Domain.Entities.Purchases;
using Domain.Entities.Roles;
using Domain.Entities.Trash;
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
        public UserSetting UserSettings { get; set; } = new ();
        public ICollection<FileActivity> FileActivities { get; set; } = new List<FileActivity>();
    }
}
