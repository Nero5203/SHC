using Domain.Entities.Auth;
using Domain.Entities.Purchase;

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

        public ICollection<Domain.Entities.Purchase.Invoice> Invoices { get; set; } = new List<Domain.Entities.Purchase.Invoice>();
        public ICollection<Domain.Entities.Purchase.Purchase> Purchases { get; set; } = new List<Domain.Entities.Purchase.Purchase>();
        public ICollection<Domain.Entities.Purchase.UserSubscription> UserSubscriptions { get; set; } = new List<Domain.Entities.Purchase.UserSubscription>();
    }
}
