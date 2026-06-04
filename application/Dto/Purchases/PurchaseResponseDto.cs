using Domain.Entities.Purchases.Enums;

namespace application.Dto.Purchases
{
    public class PurchaseResponseDto
    {
        public Guid PurchaseId { get; set; }

        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;

        public DateTime PurchasedAt { get; set; }

        public PurchaseStatus Status { get; set; }
    }
}
