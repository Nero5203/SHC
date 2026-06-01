using Domain.Entities.Purchases.Enums;

namespace Domain.Entities.Purchases
{
    public class SubscriptionPlan
    {
        public Guid SubscriptionPlanId { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public string Currency { get; set; } = "EUR";
        public BillingInterval BillingInterval { get; set; }

        public long StorageLimitBytes { get; set; }
        public long? MaxFileSizeBytes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
