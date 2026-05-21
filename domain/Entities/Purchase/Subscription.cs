using Domain.Entities.Purchase.Enums;

namespace Domain.Entities.Purchase
{
    public class Subscription
    {
        public Guid Id { get; set; }

        public Guid SubscriptionPlanId { get; set; }
        public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

        public SubscriptionStatus Status { get; set; }

        public DateTime StartedAt { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public DateTime? TrialEndsAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? EndedAt { get; set; }

        public bool AutoRenew { get; set; } = true;

        public string? ProviderSubscriptionId { get; set; }

        public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    }
}
