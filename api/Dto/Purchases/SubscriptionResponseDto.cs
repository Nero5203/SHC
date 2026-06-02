using Domain.Entities.Purchases.Enums;

namespace api.Dto.Purchases
{
    public class SubscriptionResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public DateTime? TrialEndsAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool AutoRenew { get; set; }
        public string? ProviderSubscriptionId { get; set; }
    }
}
