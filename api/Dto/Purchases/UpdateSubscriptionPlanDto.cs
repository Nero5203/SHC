using Domain.Entities.Purchases.Enums;

namespace api.Dto.Purchases
{
    public class UpdateSubscriptionPlanDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Currency { get; set; }
        public BillingInterval? BillingInterval { get; set; }
        public long? StorageLimitBytes { get; set; }
        public long? MaxFileSizeBytes { get; set; }
        public bool? IsActive { get; set; }
    }
}
