namespace application.Dto.Payments
{
    public class CreateCheckoutSessionDto
    {
        public string SuccessUrl { get; set; } = null!;
        public string CancelUrl { get; set; } = null!;
    }
}
