using Domain.Entities.User;

namespace Domain.Entities.Purchase
{
    public class UserSubscription
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Domain.Entities.User.User User { get; set; } = null!;

        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;

        public bool IsOwner { get; set; } = true;

        public DateTime AssignedAt { get; set; }
        public DateTime? RemovedAt { get; set; }
    }
}
