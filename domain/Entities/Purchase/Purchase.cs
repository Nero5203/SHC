using Domain.Entities.Purchase.Enums;
using UserEntity = Domain.Entities.User.User;

namespace Domain.Entities.Purchase
{
    public class Purchase
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public UserEntity User { get; set; } = null!;

        public Guid? SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";

        public DateTime PurchasedAt { get; set; }

        public PurchaseStatus Status { get; set; }

        public Invoice? Invoice { get; set; }
    }
}
