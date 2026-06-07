using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.UseCases.Purchases
{
    internal static class InvoiceFactory
    {
        public static Invoice CreateFromPurchase(Purchase purchase, DateTime utcNow)
        {
            var invoiceStatus = purchase.Status switch
            {
                PurchaseStatus.Paid => InvoiceStatus.Paid,
                PurchaseStatus.Refunded => InvoiceStatus.Refunded,
                PurchaseStatus.Cancelled or PurchaseStatus.Failed => InvoiceStatus.Voided,
                _ => InvoiceStatus.Issued
            };

            return new Invoice
            {
                InvoiceId = Guid.NewGuid(),
                InvoiceNumber = CreateInvoiceNumber(utcNow),
                UserId = purchase.UserId,
                PurchaseId = purchase.PurchaseId,
                SubtotalAmount = purchase.Amount,
                TaxAmount = 0,
                DiscountAmount = 0,
                TotalAmount = purchase.Amount,
                Currency = purchase.Currency,
                Status = invoiceStatus,
                IssuedAt = utcNow,
                DueAt = invoiceStatus == InvoiceStatus.Paid || invoiceStatus == InvoiceStatus.Voided
                    ? null
                    : utcNow.AddDays(7),
                PaidAt = invoiceStatus == InvoiceStatus.Paid ? utcNow : null,
                VoidedAt = invoiceStatus == InvoiceStatus.Voided ? utcNow : null
            };
        }

        private static string CreateInvoiceNumber(DateTime utcNow)
        {
            return $"INV-{utcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}";
        }
    }
}
