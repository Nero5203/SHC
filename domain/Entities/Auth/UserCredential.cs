using UserEntity = Domain.Entities.User.User;

namespace Domain.Entities.Auth
{
    public class UserCredentials
    {
        public Guid Id { get; set; }
        public string PasswordHash { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;
    }
}
