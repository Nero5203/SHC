using Domain.Entities.Purchases.Enums;

namespace application.Dto.Subscriptions
{
    public class SubscriptionResponseDto
    {
        public Guid SubscriptionId { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public string PlanName { get; set; } = null!;
        public SubscriptionStatus Status { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public DateTime? TrialEndsAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool AutoRenew { get; set; }
        public string? ProviderSubscriptionId { get; set; }
        public IReadOnlyList<Guid> UserIds { get; set; } = Array.Empty<Guid>();
    }
}
