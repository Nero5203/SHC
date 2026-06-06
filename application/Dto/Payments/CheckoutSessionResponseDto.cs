namespace application.Dto.Payments
{
    public class CheckoutSessionResponseDto
    {
        public Guid PurchaseId { get; set; }
        public string CheckoutSessionId { get; set; } = null!;
        public string CheckoutUrl { get; set; } = null!;
    }
}
