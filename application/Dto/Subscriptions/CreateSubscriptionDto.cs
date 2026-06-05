namespace application.Dto.Subscriptions
{
    public class CreateSubscriptionDto
    {
        public Guid UserId { get; set; }
        public Guid SubscriptionPlanId { get; set; }
        public DateTime? TrialEndsAt { get; set; }
        public bool AutoRenew { get; set; } = true;
        public string? ProviderSubscriptionId { get; set; }
    }
}
