using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;
using Domain.Entities.Notifications.Enums;
using application.Ports.Driven.Notifications;
using application.Ports.Driven.Purchases;
using application.Ports.Driving.Purchases;

namespace application.UseCases.Purchases
{
    public class UpdatePurchaseStatusUseCase : IUpdatePurchaseStatusUseCase
    {
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly INotificationRepository _notificationRepository;

        public UpdatePurchaseStatusUseCase(
            IPurchaseRepository purchaseRepository,
            INotificationRepository notificationRepository)
        {
            _purchaseRepository = purchaseRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task<Purchase?> ExecuteAsync(Guid purchaseId, PurchaseStatus status)
        {
            var purchase = await _purchaseRepository.GetByIdAsync(purchaseId);

            if (purchase == null)
            {
                return null;
            }

            await _purchaseRepository.UpdateStatusAsync(purchase, status);

            if (status == PurchaseStatus.Paid)
            {
                var existingInvoice = await _purchaseRepository.GetInvoiceByPurchaseIdAsync(purchaseId);
                if (existingInvoice == null)
                {
                    var invoice = InvoiceFactory.CreateFromPurchase(purchase, DateTime.UtcNow);
                    await _purchaseRepository.CreateInvoiceAsync(invoice);
                }
            }

            await CreatePurchaseStatusNotificationAsync(purchase, status);

            return purchase;
        }

        private async Task CreatePurchaseStatusNotificationAsync(Purchase purchase, PurchaseStatus status)
        {
            var (title, message) = status switch
            {
                PurchaseStatus.Paid => (
                    "Payment received",
                    "Your payment was marked as paid and your invoice is available."
                ),
                PurchaseStatus.Failed => (
                    "Payment failed",
                    "A payment attempt for your subscription failed."
                ),
                PurchaseStatus.Cancelled => (
                    "Payment cancelled",
                    "A subscription purchase was cancelled."
                ),
                PurchaseStatus.Refunded => (
                    "Payment refunded",
                    "A subscription payment was refunded."
                ),
                _ => (string.Empty, string.Empty)
            };

            if (string.IsNullOrWhiteSpace(title))
            {
                return;
            }

            await _notificationRepository.CreateAsync(new global::Notification
            {
                NotificationId = Guid.NewGuid(),
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                Type = NotificationType.SystemAlert,
                Channel = NotificationChannel.InApp,
                UserId = purchase.UserId
            });
        }
    }
}
