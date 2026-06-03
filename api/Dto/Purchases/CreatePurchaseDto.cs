namespace api.Dto.Purchases
{
    public class CreatePurchaseDto
    {
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";
    }
}
