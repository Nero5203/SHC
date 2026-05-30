using Domain.Entities.Users;

namespace Domain.Entities.Auth
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; } = string.Empty;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;


        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}