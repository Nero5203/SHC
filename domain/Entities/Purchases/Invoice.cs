using Domain.Entities.Purchases.Enums;
using Domain.Entities.Users;

namespace Domain.Entities.Purchases
{
    public class Invoice
    {
        public Guid InvoiceId { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;


        public Guid PurchaseId { get; set; }
        public Purchase Purchase { get; set; } = null!;

        public decimal SubtotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "EUR";

        public InvoiceStatus Status { get; set; }

        public DateTime IssuedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? VoidedAt { get; set; }

        public string? ProviderInvoiceId { get; set; }
    }
}
