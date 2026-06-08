using Domain.Entities.Purchases.Enums;

namespace application.Ports.Driven.Payments
{
    public interface IPaymentGatewayService
    {
        Task<CheckoutSessionResult> CreateCheckoutSessionAsync(PaymentCheckoutRequest request);
        Task<CheckoutSessionPaymentResult> GetCheckoutSessionPaymentAsync(string checkoutSessionId);
        Task<PaymentGatewayWebhookResult> ProcessWebhookAsync(string payload, string signatureHeader);
    }

    public class PaymentCheckoutRequest
    {
        public Guid PurchaseId { get; set; }
        public Guid UserId { get; set; }
        public Guid SubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EUR";
        public string SuccessUrl { get; set; } = null!;
        public string CancelUrl { get; set; } = null!;
    }

    public class CheckoutSessionResult
    {
        public string CheckoutSessionId { get; set; } = null!;
        public string CheckoutUrl { get; set; } = null!;
    }

    public class CheckoutSessionPaymentResult
    {
        public bool IsPaid { get; set; }
        public string? ProviderPaymentIntentId { get; set; }
    }

    public class PaymentGatewayWebhookResult
    {
        public bool Handled { get; set; }
        public Guid? PurchaseId { get; set; }
        public PurchaseStatus? PurchaseStatus { get; set; }
        public string? ProviderPaymentIntentId { get; set; }
    }
}
