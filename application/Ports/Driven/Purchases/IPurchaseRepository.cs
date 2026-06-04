using Domain.Entities.Purchases;
using Domain.Entities.Purchases.Enums;

namespace application.Ports.Driven.Purchases
{
    public interface IPurchaseRepository
    {
        Task CreateAsync(Purchase purchase);
        Task<Purchase?> GetByIdAsync(Guid purchaseId);
        Task<IReadOnlyList<Purchase>> GetByUserIdAsync(Guid userId);
        Task<Invoice?> GetInvoiceByPurchaseIdAsync(Guid purchaseId);
        Task UpdateStatusAsync(Purchase purchase, PurchaseStatus status);
    }
}
