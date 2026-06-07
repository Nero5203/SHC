namespace application.Dto.Purchases
{
    public class CreatePurchaseDto
    {
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }
    }
}
