using application.Ports.Driven.Notifications;
using application.Ports.Driven.Payments;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Payments;
using application.UseCases.Purchases;
using application.UseCases.Subscriptions;
using Domain.Entities.Notifications.Enums;
using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Payments
{
    public class ConfirmCheckoutPaymentUseCase : IConfirmCheckoutPaymentUseCase
    {
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationRepository _notificationRepository;

        public ConfirmCheckoutPaymentUseCase(
            IPaymentGatewayService paymentGatewayService,
            IPurchaseRepository purchaseRepository,
            ISubscriptionRepository subscriptionRepository,
            INotificationRepository notificationRepository)
        {
            _paymentGatewayService = paymentGatewayService;
            _purchaseRepository = purchaseRepository;
            _subscriptionRepository = subscriptionRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<Purchase?> ExecuteAsync(Guid purchaseId)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(purchaseId);

            if (purchase == null)
            {
                return null;
            }

            if (purchase.Status == PurchaseStatus.Paid)
            {
                return purchase;
            }

            if (string.IsNullOrWhiteSpace(purchase.ProviderCheckoutSessionId))
            {
                throw new InvalidOperationException("Purchase does not have a Stripe checkout session to confirm.");
            }

            var checkoutPayment = await _paymentGatewayService
                .GetCheckoutSessionPaymentAsync(purchase.ProviderCheckoutSessionId);

            if (!checkoutPayment.IsPaid)
            {
                return purchase;
            }

            if (!string.IsNullOrWhiteSpace(checkoutPayment.ProviderPaymentIntentId))
            {
                purchase.ProviderPaymentIntentId = checkoutPayment.ProviderPaymentIntentId;
            }

            await _purchaseRepository.UpdateStatusAsync(purchase, PurchaseStatus.Paid);
            await ActivateSubscriptionAsync(purchase.SubscriptionId);
            await GenerateInvoiceIfMissingAsync(purchase);
            await CreatePaymentNotificationAsync(purchase);

            return purchase;
        }

        private async Task ActivateSubscriptionAsync(Guid subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetSubscriptionByIdAsync(subscriptionId);

            if (subscription == null)
            {
                return;
            }

            var now = DateTime.UtcNow;
            subscription.StartedAt = now;
            subscription.CurrentPeriodStart = now;
            subscription.CurrentPeriodEnd = SubscriptionPeriodCalculator.AddBillingPeriod(
                now,
                subscription.SubscriptionPlan.BillingInterval);
            subscription.Status = subscription.TrialEndsAt.HasValue && subscription.TrialEndsAt.Value > now
                ? SubscriptionStatus.Trialing
                : SubscriptionStatus.Active;

            await _subscriptionRepository.UpdateSubscriptionAsync(subscription);
        }

        private async Task GenerateInvoiceIfMissingAsync(Purchase purchase)
        {
            var existingInvoice = await _purchaseRepository.GetInvoiceByPurchaseIdAsync(purchase.PurchaseId);

            if (existingInvoice != null)
            {
                return;
            }

            var invoice = InvoiceFactory.CreateFromPurchase(purchase, DateTime.UtcNow);
            await _purchaseRepository.CreateInvoiceAsync(invoice);
        }

        private async Task CreatePaymentNotificationAsync(Purchase purchase)
        {
            await _notificationRepository.CreateAsync(new global::Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = "Payment received",
                Message = "Your payment was successful. Your subscription is now active.",
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Type = NotificationType.SystemAlert,
                Channel = NotificationChannel.InApp,
                UserId = purchase.UserId
            });
        }
    }
}
