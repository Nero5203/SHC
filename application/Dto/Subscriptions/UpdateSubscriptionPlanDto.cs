using Domain.Entities.Purchases.Enums;

namespace application.Dto.Subscriptions
{
    public class UpdateSubscriptionPlanDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "EUR";
        public BillingInterval BillingInterval { get; set; }
        public long StorageLimitBytes { get; set; }
        public long? MaxFileSizeBytes { get; set; }
        public bool IsActive { get; set; }
    }
}
