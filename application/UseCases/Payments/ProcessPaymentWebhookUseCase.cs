using application.Ports.Driven.Payments;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Payments;

namespace application.UseCases.Payments
{
    public class ProcessPaymentWebhookUseCase : IProcessPaymentWebhookUseCase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IPurchaseRepository _purchaseRepository;

        public ProcessPaymentWebhookUseCase(
            IPaymentGatewayService paymentGatewayService,
            IPurchaseRepository purchaseRepository)
        {
            _paymentGatewayService = paymentGatewayService;
            _purchaseRepository = purchaseRepository;
        }

        public async Task<bool> ExecuteAsync(string payload, string signatureHeader)
        {
            var webhookResult = await _paymentGatewayService.ProcessWebhookAsync(payload, signatureHeader);

            if (!webhookResult.Handled)
            {
                return false;
            }

            if (webhookResult.PurchaseId == null || webhookResult.PurchaseStatus == null)
            {
                return true; // valid Stripe event, but not one we need to act on
            }

            var purchase = await _purchaseRepository.GetByIdAsync(webhookResult.PurchaseId.Value);
            if (purchase == null)
            {
                return false;
            }

            if (!string.IsNullOrWhiteSpace(webhookResult.ProviderPaymentIntentId))
            {
                purchase.ProviderPaymentIntentId = webhookResult.ProviderPaymentIntentId;
            }

            await _purchaseRepository.UpdateStatusAsync(purchase, webhookResult.PurchaseStatus.Value);
            return true;
        }
    }
}
