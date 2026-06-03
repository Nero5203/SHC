using Domain.Entities.Users;

namespace Domain.Entities.Auth
{
    public class UserCredential
    {
        public Guid UserCredentialId { get; set; }
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
