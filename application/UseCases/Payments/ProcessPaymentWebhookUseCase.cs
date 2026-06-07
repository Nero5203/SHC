using application.Ports.Driven.Payments;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Payments;
using application.UseCases.Subscriptions;
using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Payments
{
    public class ProcessPaymentWebhookUseCase : IProcessPaymentWebhookUseCase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ProcessPaymentWebhookUseCase(
            IPaymentGatewayService paymentGatewayService,
            IPurchaseRepository purchaseRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _paymentGatewayService = paymentGatewayService;
            _purchaseRepository = purchaseRepository;
            _subscriptionRepository = subscriptionRepository;
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
            await UpdateSubscriptionStatusAsync(purchase.SubscriptionId, webhookResult.PurchaseStatus.Value);
            return true;
        }

        private async Task UpdateSubscriptionStatusAsync(Guid subscriptionId, PurchaseStatus purchaseStatus)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);
            if (subscription == null)
            {
                return;
            }

            if (purchaseStatus == PurchaseStatus.Paid)
            {
                var now = DateTime.UtcNow;
                subscription.StartedAt = now;
                subscription.CurrentPeriodStart = now;
                subscription.CurrentPeriodEnd = SubscriptionPeriodCalculator.AddBillingPeriod(
                    now,
                    subscription.SubscriptionPlan.BillingInterval);
                subscription.Status = subscription.TrialEndsAt.HasValue && subscription.TrialEndsAt.Value > now
                    ? SubscriptionStatus.Trialing
                    : SubscriptionStatus.Active;
            }
            else if (purchaseStatus == PurchaseStatus.Cancelled || purchaseStatus == PurchaseStatus.Failed)
            {
                subscription.Status = SubscriptionStatus.Pending;
            }

            await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
        }
    }
}
