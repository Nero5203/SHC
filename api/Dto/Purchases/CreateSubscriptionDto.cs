namespace api.Dto.Purchases
{
    public class CreateSubscriptionDto
    {
        public Guid SubscriptionPlanId { get; set; }
        public Guid UserId { get; set; }
    }
}
