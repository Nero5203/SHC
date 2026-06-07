using application.Ports.Driven.Payments;
using application.Ports.Driven.Notifications;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Payments;
using application.UseCases.Purchases;
using application.UseCases.Subscriptions;
using Domain.Entities.Notifications.Enums;
using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Payments
{
    public class ProcessPaymentWebhookUseCase : IProcessPaymentWebhookUseCase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ProcessPaymentWebhookUseCase(
            IPaymentGatewayService paymentGatewayService,
            INotificationRepository notificationRepository,
            IPurchaseRepository purchaseRepository,
            ISubscriptionRepository subscriptionRepository)
        {
            _paymentGatewayService = paymentGatewayService;
            _notificationRepository = notificationRepository;
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

            var previousStatus = purchase.Status;
            await _purchaseRepository.UpdateStatusAsync(purchase, webhookResult.PurchaseStatus.Value);
            await UpdateSubscriptionStatusAsync(purchase.SubscriptionId, webhookResult.PurchaseStatus.Value);
            await GenerateInvoiceIfPaidAsync(purchase, webhookResult.PurchaseStatus.Value);

            if (previousStatus != webhookResult.PurchaseStatus.Value)
            {
                await CreateBillingNotificationAsync(purchase, webhookResult.PurchaseStatus.Value);
            }

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

        private async Task GenerateInvoiceIfPaidAsync(
            Domain.Entities.Purchases.Purchase purchase,
            PurchaseStatus purchaseStatus)
        {
            if (purchaseStatus != PurchaseStatus.Paid)
            {
                return;
            }

            var existingInvoice = await _purchaseRepository.GetInvoiceByPurchaseIdAsync(purchase.PurchaseId);
            if (existingInvoice != null)
            {
                return;
            }

            var invoice = InvoiceFactory.CreateFromPurchase(purchase, DateTime.UtcNow);
            await _purchaseRepository.CreateInvoiceAsync(invoice);
        }

        private async Task CreateBillingNotificationAsync(
            Domain.Entities.Purchases.Purchase purchase,
            PurchaseStatus purchaseStatus)
        {
            if (purchaseStatus != PurchaseStatus.Paid
                && purchaseStatus != PurchaseStatus.Failed
                && purchaseStatus != PurchaseStatus.Cancelled)
            {
                return;
            }

            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(purchase.SubscriptionId);
            var planName = subscription?.SubscriptionPlan?.Name ?? "your subscription";

            var (title, message) = purchaseStatus switch
            {
                PurchaseStatus.Paid => (
                    "Payment received",
                    $"Your payment for {planName} was successful. Your subscription is now active."
                ),
                PurchaseStatus.Failed => (
                    "Payment failed",
                    $"We could not complete the payment for {planName}. Please try again to activate your subscription."
                ),
                PurchaseStatus.Cancelled => (
                    "Checkout cancelled",
                    $"Your checkout for {planName} was cancelled before payment completed."
                ),
                _ => (string.Empty, string.Empty)
            };

            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            var notification = new global::Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Type = NotificationType.SystemAlert,
                Channel = NotificationChannel.InApp,
                UserId = purchase.UserId
            };

            await _notificationRepository.CreateAsync(notification);
        }
    }
}
