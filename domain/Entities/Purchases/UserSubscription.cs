using Domain.Entities.Users;

namespace Domain.Entities.Purchases
{
    public class UserSubscription
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;

        public bool IsOwner { get; set; } = true;

        public DateTime AssignedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
    }
}
