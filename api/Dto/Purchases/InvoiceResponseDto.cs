using Domain.Entities.Purchases.Enums;

namespace api.Dto.Purchases
{
    public class InvoiceResponseDto
    {
        public Guid InvoiceId { get; set; }

        public string InvoiceNumber { get; set; } = null!;

        public Guid UserId { get; set; }
        public Guid PurchaseId { get; set; }

        public decimal SubtotalAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = null!;

        public InvoiceStatus Status { get; set; }

        public DateTime IssuedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? VoidedAt { get; set; }

        public string? ProviderInvoiceId { get; set; }
    }
}
