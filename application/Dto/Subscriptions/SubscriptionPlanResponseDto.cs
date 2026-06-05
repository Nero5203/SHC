using Domain.Entities.Purchases.Enums;

namespace application.Dto.Subscriptions
{
    public class SubscriptionPlanResponseDto
    {
        public Guid SubscriptionPlanId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = null!;
        public BillingInterval BillingInterval { get; set; }
        public long StorageLimitBytes { get; set; }
        public long? MaxFileSizeBytes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
