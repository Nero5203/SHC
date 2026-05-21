using Domain.Entities.Purchase.Enums;
using Domain.Entities.User;

namespace Domain.Entities.Purchase
{
    public class Invoice
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public Guid UserId { get; set; }
        public Domain.Entities.User.User User { get; set; } = null!;


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
