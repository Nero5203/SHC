using Domain.Entities.Purchases.Enums;

namespace application.Dto.Subscriptions
{
    public class SubscriptionEntitlementResponseDto
    {
        public Guid UserId { get; set; }
        public Guid? SubscriptionId { get; set; }
        public Guid? SubscriptionPlanId { get; set; }
        public string? PlanName { get; set; }
        public SubscriptionStatus? Status { get; set; }
        public bool HasActiveSubscription { get; set; }
        public long StorageLimitBytes { get; set; }
        public long? MaxFileSizeBytes { get; set; }
        public DateTime? CurrentPeriodEnd { get; set; }
    }
}
