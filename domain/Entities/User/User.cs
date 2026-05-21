using Domain.Entities.Auth;
namespace Domain.Entities.User
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        
        public string? ProfilePictureUrl { get; set; }
        public string? PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserCredentials Credentials { get; set; } = null!;
        public UserSettings Settings { get; set; } = new ();
    }
}